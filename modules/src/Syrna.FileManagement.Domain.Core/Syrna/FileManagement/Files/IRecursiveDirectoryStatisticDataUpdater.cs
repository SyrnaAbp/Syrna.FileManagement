using Volo.Abp.EventBus.Distributed;

namespace Syrna.FileManagement.Files;

public interface IRecursiveDirectoryStatisticDataUpdater : IDistributedEventHandler<SubFilesChangedEto>
{
}