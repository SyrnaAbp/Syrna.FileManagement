using Syrna.FileManagement.UnifiedDemo.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Syrna.FileManagement.UnifiedDemo.Blazor.Server.Host
{
    public abstract class UnifiedDemoComponentBase : AbpComponentBase
    {
        protected UnifiedDemoComponentBase()
        {
            LocalizationResource = typeof(UnifiedDemoResource);
        }
    }
}
