using Syrna.FileManagement.UnifiedDemo.Blazor;
using Volo.Abp.Modularity;

namespace Syrna.FileManagement.UnifiedDemo.Blazor.WebAssembly;

[DependsOn(
    typeof(UnifiedDemoBlazorModule)
)]
public class UnifiedDemoBlazorWebAssemblyModule : AbpModule
{
}
