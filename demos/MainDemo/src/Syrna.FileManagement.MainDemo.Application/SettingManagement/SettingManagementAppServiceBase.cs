using Syrna.FileManagement.MainDemo;
using Syrna.FileManagement.MainDemo.Localization;
using Volo.Abp.Application.Services;

namespace Syrna.FileManagement.MainDemo.SettingManagement;

public abstract class SettingManagementAppServiceBase : ApplicationService
{
    protected SettingManagementAppServiceBase()
    {
        ObjectMapperContext = typeof(MainDemoApplicationModule);
        LocalizationResource = typeof(MainDemoResource);
    }
}
