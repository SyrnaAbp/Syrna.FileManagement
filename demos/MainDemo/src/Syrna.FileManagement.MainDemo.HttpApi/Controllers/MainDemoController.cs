using Syrna.FileManagement.MainDemo.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Syrna.FileManagement.MainDemo.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MainDemoController : AbpControllerBase
{
    protected MainDemoController()
    {
        LocalizationResource = typeof(MainDemoResource);
    }
}
