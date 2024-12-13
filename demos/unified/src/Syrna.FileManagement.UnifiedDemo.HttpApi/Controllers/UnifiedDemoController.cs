using Syrna.FileManagement.UnifiedDemo.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Syrna.FileManagement.UnifiedDemo.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class UnifiedDemoController : AbpControllerBase
{
    protected UnifiedDemoController()
    {
        LocalizationResource = typeof(UnifiedDemoResource);
    }
}
