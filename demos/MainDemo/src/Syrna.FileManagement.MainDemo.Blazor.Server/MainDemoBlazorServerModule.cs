using Volo.Abp.Modularity;

namespace Syrna.FileManagement.MainDemo.Blazor.Server;

[DependsOn(
    typeof(MainDemoBlazorModule)
)]
public class MainDemoBlazorServerModule : AbpModule
{

}
