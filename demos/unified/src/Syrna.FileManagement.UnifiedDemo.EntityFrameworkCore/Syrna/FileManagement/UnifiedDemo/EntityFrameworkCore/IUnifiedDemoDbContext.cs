using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Syrna.FileManagement;

namespace Syrna.FileManagement.UnifiedDemo.EntityFrameworkCore
{
    [ConnectionStringName(FileManagementDbProperties.ConnectionStringName)]
    public interface IUnifiedDemoDbContext : IEfCoreDbContext
    {
    }
}
