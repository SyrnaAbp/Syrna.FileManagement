﻿using EasyAbp.Abp.TagHelperPlus;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Syrna.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Syrna.FileManagement.Web
{
    [DependsOn(
        typeof(FileManagementApplicationContractsModule),
        typeof(AbpAspNetCoreMvcUiThemeSharedModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpTagHelperPlusModule)
    )]
    public class FileManagementWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(typeof(FileManagementResource), typeof(FileManagementWebModule).Assembly);
            });

            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(FileManagementWebModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<FileManagementWebModule>();
            });

            context.Services.AddAutoMapperObjectMapper<FileManagementWebModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<FileManagementWebModule>(validate: true);
            });

            Configure<RazorPagesOptions>(options =>
            {
                //Configure authorization.
            });
        }
    }
}
