using Syrna.FileManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Syrna.FileManagement.MainDemo.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class MainDemoDbContext(DbContextOptions<MainDemoDbContext> options) : AbpDbContext<MainDemoDbContext>(options), IMainDemoDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureFileManagement();
    }
}
