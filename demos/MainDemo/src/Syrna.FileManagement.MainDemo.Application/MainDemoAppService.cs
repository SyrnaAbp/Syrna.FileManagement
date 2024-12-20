using Syrna.FileManagement.MainDemo.Localization;
using Volo.Abp.Application.Services;

namespace Syrna.FileManagement.MainDemo;

/* Inherit your application services from this class.
 */
public abstract class MainDemoAppService : ApplicationService
{
    protected MainDemoAppService()
    {
        LocalizationResource = typeof(MainDemoResource);
    }
}
