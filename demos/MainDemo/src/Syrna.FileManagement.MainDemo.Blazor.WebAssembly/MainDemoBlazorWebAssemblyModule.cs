using Syrna.FileManagement.MainDemo.Blazor;
using Volo.Abp.Modularity;

namespace Syrna.FileManagement.MainDemo.Blazor.WebAssembly;

[DependsOn(
    typeof(MainDemoBlazorModule)
)]
public class MainDemoBlazorWebAssemblyModule : AbpModule
{
}
