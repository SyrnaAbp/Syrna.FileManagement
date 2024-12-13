using Syrna.FileManagement.Localization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Syrna.FileManagement
{
    [Area(FileManagementRemoteServiceConsts.ModuleName)]
    public abstract class FileManagementController : AbpControllerBase
    {
        protected FileManagementController()
        {
            LocalizationResource = typeof(FileManagementResource);
        }
    }
}
