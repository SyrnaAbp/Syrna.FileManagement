using Syrna.FileManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Syrna.FileManagement.UnifiedDemo.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class UnifiedDemoDbContext(DbContextOptions<UnifiedDemoDbContext> options) : AbpDbContext<UnifiedDemoDbContext>(options), IUnifiedDemoDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureFileManagement();
    }
}
