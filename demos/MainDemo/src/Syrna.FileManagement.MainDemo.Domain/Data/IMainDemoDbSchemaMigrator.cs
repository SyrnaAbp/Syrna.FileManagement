using System.Threading.Tasks;

namespace Syrna.FileManagement.MainDemo.Data;

public interface IMainDemoDbSchemaMigrator
{
    Task MigrateAsync();
}
