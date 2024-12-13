using Volo.Abp.Modularity;

namespace Syrna.FileManagement.UnifiedDemo.Blazor.Server;

[DependsOn(
    typeof(UnifiedDemoBlazorModule)
)]
public class UnifiedDemoBlazorServerModule : AbpModule
{

}
