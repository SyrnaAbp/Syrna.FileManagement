using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using Volo.Abp.Authorization;

namespace Syrna.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainCoreModule),
        typeof(FileManagementApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
        )]
    //[DependsOn(typeof(AbpAuthorizationModule))]
    public class FileManagementApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<FileManagementApplicationModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<FileManagementApplicationModule>(validate: true);
            });
        }
    }
}
