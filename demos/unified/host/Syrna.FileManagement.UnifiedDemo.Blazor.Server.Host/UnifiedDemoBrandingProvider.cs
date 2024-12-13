using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Syrna.FileManagement.UnifiedDemo.Blazor.Server.Host
{
    [Dependency(ReplaceServices = true)]
    public class UnifiedDemoBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "FileManagement";
    }
}
