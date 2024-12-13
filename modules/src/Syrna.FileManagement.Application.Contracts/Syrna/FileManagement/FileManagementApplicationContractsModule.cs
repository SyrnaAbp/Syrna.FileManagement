using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Syrna.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class FileManagementApplicationContractsModule : AbpModule
    {

    }
}
