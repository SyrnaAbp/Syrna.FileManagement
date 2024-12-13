using Syrna.FileManagement.UnifiedDemo.Localization;
using Volo.Abp.Application.Services;

namespace Syrna.FileManagement.UnifiedDemo;

/* Inherit your application services from this class.
 */
public abstract class UnifiedDemoAppService : ApplicationService
{
    protected UnifiedDemoAppService()
    {
        LocalizationResource = typeof(UnifiedDemoResource);
    }
}
