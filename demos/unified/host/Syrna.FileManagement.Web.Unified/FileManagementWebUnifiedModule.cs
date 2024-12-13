using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syrna.FileManagement.UnifiedDemo;
using Syrna.FileManagement.UnifiedDemo.EntityFrameworkCore;
using Syrna.FileManagement.UnifiedDemo.Web;
using Microsoft.OpenApi.Models;
using Syrna.FileManagement.UnifiedDemo.Localization;
using Syrna.FileManagement.UnifiedDemo.MultiTenancy;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AutoMapper;
using Volo.Abp.Security.Claims;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI.Navigation;
using Microsoft.Extensions.Configuration;
using Syrna.FileManagement.Containers;
using Syrna.FileManagement.Files;
using Syrna.FileManagement.Menus;
using Syrna.FileManagement.Options;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.Authorization;

namespace Syrna.FileManagement
{
    [DependsOn(typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule))]

    [DependsOn(typeof(AbpAutofacModule))]
    [DependsOn(typeof(AbpAspNetCoreSerilogModule))]
    [DependsOn(typeof(AbpSwashbuckleModule))]
    [DependsOn(typeof(AbpBlobStoringFileSystemModule))]

    [DependsOn(typeof(UnifiedDemoHttpApiModule))]
    [DependsOn(typeof(UnifiedDemoEntityFrameworkCoreModule))]
    [DependsOn(typeof(UnifiedDemoApplicationModule))]
    [DependsOn(typeof(UnifiedDemoWebModule))]
    public class FileManagementWebUnifiedModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(
                    typeof(UnifiedDemoResource),
                    typeof(UnifiedDemoDomainModule).Assembly,
                    typeof(UnifiedDemoDomainSharedModule).Assembly,
                    typeof(UnifiedDemoApplicationModule).Assembly,
                    typeof(UnifiedDemoApplicationContractsModule).Assembly,
                    typeof(UnifiedDemoWebModule).Assembly
                );
            });

            PreConfigure<OpenIddictBuilder>(builder =>
            {
                builder.AddValidation(options =>
                {
                    options.AddAudiences("FileManagement");
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });
            });

            if (!hostingEnvironment.IsDevelopment())
            {
                PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
                {
                    options.AddDevelopmentEncryptionAndSigningCertificate = false;
                });

                PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
                {
                    serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", "00000000-0000-0000-0000-000000000000");
                });
            }
        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            ConfigureAuthentication(context);
            ConfigureUrls(configuration);
            ConfigureBundles();
            ConfigureAutoMapper();
            ConfigureVirtualFileSystem(hostingEnvironment);
            ConfigureNavigationServices();
            ConfigureAutoApiControllers();
            ConfigureSwaggerServices(context.Services);

            Configure<AbpBlobStoringOptions>(options =>
            {
                options.Containers.Configure<LocalFileSystemBlobContainer>(container =>
                {
                    container.IsMultiTenant = true;
                    container.UseFileSystem(fileSystem =>
                    {
                        // fileSystem.BasePath = "C:\\my-files";
                        fileSystem.BasePath = Path.Combine(hostingEnvironment.ContentRootPath, "my-files");
                    });
                });
            });

            Configure<FileManagementOptions>(options =>
            {
                options.DefaultFileDownloadProviderType = typeof(LocalFileDownloadProvider);
                options.Containers.Configure<CommonFileContainer>(container =>
                {
                    // private container never be used by non-owner users (except user who has the "File.Manage" permission).
                    container.FileContainerType = FileContainerType.Public;
                    container.AbpBlobContainerName = BlobContainerNameAttribute.GetContainerName<LocalFileSystemBlobContainer>();
                    container.AbpBlobDirectorySeparator = "/";

                    container.RetainUnusedBlobs = false;
                    container.EnableAutoRename = true;

                    container.MaxByteSizeForEachFile = 5 * 1024 * 1024;
                    container.MaxByteSizeForEachUpload = 10 * 1024 * 1024;
                    container.MaxFileQuantityForEachUpload = 2;

                    container.AllowOnlyConfiguredFileExtensions = true;
                    container.FileExtensionsConfiguration.Add(".jpg", true);
                    container.FileExtensionsConfiguration.Add(".PNG", true);
                    // container.FileExtensionsConfiguration.Add(".tar.gz", true);
                    // container.FileExtensionsConfiguration.Add(".exe", false);

                    container.GetDownloadInfoTimesLimitEachUserPerMinute = 10;
                });
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context)
        {
            context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
            context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
            {
                options.IsDynamicClaimsEnabled = true;
            });
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            });
        }

        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>(options =>
            {
                options.StyleBundles.Configure(
                    LeptonXLiteThemeBundles.Styles.Global,
                    bundle =>
                    {
                        bundle.AddFiles("/global-styles.css");
                    }
                );
            });
        }

        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<FileManagementWebUnifiedModule>();
            });
        }

        private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Domain.Shared", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Domain", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Application.Contracts", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Application", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoWebModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Web", Path.DirectorySeparatorChar)));
                });
            }
        }

        private void ConfigureNavigationServices()
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new MyMenuContributor());
            });
        }

        private void ConfigureAutoApiControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(FileManagementApplicationModule).Assembly);
            });
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddAbpSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FileManagement API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                }
            );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }

            app.UseCorrelationId();
            app.MapAbpStaticAssets();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAbpOpenIddictValidation();

            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }

            app.UseUnitOfWork();
            app.UseDynamicClaims();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FileManagement API");
            });

            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }

        //public override void ConfigureServices(ServiceConfigurationContext context)
        //{
        //    var hostingEnvironment = context.Services.GetHostingEnvironment();
        //    var configuration = context.Services.GetConfiguration();

        //    PreConfigure<AbpAspNetCoreMvcOptions>(options =>
        //    {
        //        options
        //            .ConventionalControllers
        //            .Create(typeof(FileManagementApplicationModule).Assembly, conventionalControllerSetting =>
        //            {
        //                conventionalControllerSetting.ApplicationServiceTypes =
        //                    ApplicationServiceTypes.IntegrationServices;
        //            });
        //    });

        //    Configure<AbpDbContextOptions>(options =>
        //    {
        //        options.UseSqlServer();
        //    });

        //    if (hostingEnvironment.IsDevelopment())
        //    {
        //        Configure<AbpVirtualFileSystemOptions>(options =>
        //        {
        //            options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Domain.Shared", Path.DirectorySeparatorChar)));
        //            options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Domain", Path.DirectorySeparatorChar)));
        //            options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Application.Contracts", Path.DirectorySeparatorChar)));
        //            options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Application", Path.DirectorySeparatorChar)));
        //            options.FileSets.ReplaceEmbeddedByPhysical<UnifiedDemoWebModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Syrna.FileManagement.UnifiedDemo.Web", Path.DirectorySeparatorChar)));
        //        });
        //    }

        //    context.Services.AddSwaggerGen(
        //        options =>
        //        {
        //            options.SwaggerDoc("v1", new OpenApiInfo { Title = "FileManagement API", Version = "v1" });
        //            options.DocInclusionPredicate((docName, description) => true);
        //            options.CustomSchemaIds(type => type.FullName);
        //        });

        //    Configure<AbpMultiTenancyOptions>(options =>
        //    {
        //        options.IsEnabled = MultiTenancyConsts.IsEnabled;
        //    });

        //    ConfigureConventionalControllers();
        //}

        //public override void OnApplicationInitialization(ApplicationInitializationContext context)
        //{
        //    var app = context.GetApplicationBuilder();

        //    if (context.GetEnvironment().IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        app.UseErrorPage();
        //        app.UseHsts();
        //    }

        //    app.UseHttpsRedirection();
        //    app.UseStaticFiles();
        //    app.UseRouting();
        //    //app.UseIdentityServer();
        //    app.UseAbpOpenIddictValidation();
        //    app.UseAuthentication();
        //    app.UseAuthorization();
        //    if (MultiTenancyConsts.IsEnabled)
        //    {
        //        app.UseMultiTenancy();
        //    }

        //    app.UseSwagger();
        //    app.UseSwaggerUI(options =>
        //    {
        //        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support APP API");
        //    });

        //    app.UseAbpRequestLocalization();
        //    app.UseAuditing();
        //    app.UseAbpSerilogEnrichers();
        //    app.UseConfiguredEndpoints();
        //}

        //private void ConfigureConventionalControllers()
        //{
        //    PreConfigure<AbpAspNetCoreMvcOptions>(options =>
        //    {
        //    });
        //}
    }
}
