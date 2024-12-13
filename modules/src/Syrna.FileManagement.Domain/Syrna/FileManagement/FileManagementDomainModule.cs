using Volo.Abp.BlobStoring;
using Volo.Abp.Modularity;

namespace Syrna.FileManagement
{
    [DependsOn(
        typeof(FileManagementDomainCoreModule),
        typeof(AbpBlobStoringModule)
    )]
    public class FileManagementDomainModule : AbpModule
    {
    }
}