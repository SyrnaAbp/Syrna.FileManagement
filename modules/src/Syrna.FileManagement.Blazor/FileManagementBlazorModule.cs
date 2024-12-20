﻿using Microsoft.Extensions.DependencyInjection;
using Syrna.FileManagement.Blazor.Menus;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace Syrna.FileManagement.Blazor
{
    [DependsOn(
        typeof(FileManagementApplicationContractsModule),
        typeof(AbpAspNetCoreComponentsWebThemingModule),
        typeof(AbpAutoMapperModule)
        )]
    public class FileManagementBlazorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<FileManagementBlazorModule>();

            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddProfile<FileManagementBlazorAutoMapperProfile>(validate: false);
            });

            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new FileManagementMenuContributor());
            });

            Configure<AbpRouterOptions>(options =>
            {
                options.AdditionalAssemblies.Add(typeof(FileManagementBlazorModule).Assembly);
            });
        }
    }
}