using Syrna.FileManagement.Localization;
using Volo.Abp.Application.Services;

namespace Syrna.FileManagement
{
    public abstract class FileManagementAppService : ApplicationService
    {
        protected FileManagementAppService()
        {
            LocalizationResource = typeof(FileManagementResource);
            ObjectMapperContext = typeof(FileManagementApplicationModule);
        }
    }
}
