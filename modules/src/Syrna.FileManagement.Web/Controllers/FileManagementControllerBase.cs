using Syrna.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Syrna.FileManagement.Web.Controllers;

public class FileManagementControllerBase : AbpController
{
    public FileManagementControllerBase()
    {
        LocalizationResource = typeof(FileManagementResource);
    }
}