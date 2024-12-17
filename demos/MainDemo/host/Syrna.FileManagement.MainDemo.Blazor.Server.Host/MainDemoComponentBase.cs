using Syrna.FileManagement.MainDemo.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Syrna.FileManagement.MainDemo.Blazor.Server.Host
{
    public abstract class MainDemoComponentBase : AbpComponentBase
    {
        protected MainDemoComponentBase()
        {
            LocalizationResource = typeof(MainDemoResource);
        }
    }
}
