using System.Threading.Tasks;

namespace Syrna.FileManagement.UnifiedDemo.Data;

public interface IUnifiedDemoDbSchemaMigrator
{
    Task MigrateAsync();
}
