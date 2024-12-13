using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Syrna.FileManagement.UnifiedDemo.SettingManagement;

public interface IUnifiedDemoSettingsAppService : IApplicationService
{
    Task<UnifiedDemoSettingsDto> GetAsync();

    Task UpdateAsync(UpdateUnifiedDemoSettingsDto input);
}
