using Localization.Resources.AbpUi;
using Syrna.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Syrna.FileManagement.Files.Dtos;

namespace Syrna.FileManagement
{
    [DependsOn(
        typeof(FileManagementApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class FileManagementHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(FileManagementHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<FileManagementResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });

            PreConfigure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateFileWithStreamInput));
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateManyFileWithStreamInput));
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(UpdateFileWithStreamInput));
            });
        }
    }
}
