using Syrna.FileManagement.UnifiedDemo;
using Syrna.FileManagement.UnifiedDemo.Localization;
using Volo.Abp.Application.Services;

namespace Syrna.FileManagement.UnifiedDemo.SettingManagement;

public abstract class SettingManagementAppServiceBase : ApplicationService
{
    protected SettingManagementAppServiceBase()
    {
        ObjectMapperContext = typeof(UnifiedDemoApplicationModule);
        LocalizationResource = typeof(UnifiedDemoResource);
    }
}
