using Volo.Abp.Modularity;

namespace Syrna.FileManagement
{
    [DependsOn(
        typeof(FileManagementApplicationModule),
        typeof(FileManagementDomainTestModule)
        )]
    public class FileManagementApplicationTestModule : AbpModule
    {

    }
}
