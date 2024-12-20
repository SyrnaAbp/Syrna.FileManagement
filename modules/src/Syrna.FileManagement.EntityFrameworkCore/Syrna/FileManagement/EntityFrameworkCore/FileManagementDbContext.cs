using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Syrna.FileManagement.Files;
using Syrna.FileManagement.Users;

namespace Syrna.FileManagement.EntityFrameworkCore
{
    [ConnectionStringName(FileManagementDbProperties.ConnectionStringName)]
    public class FileManagementDbContext : AbpDbContext<FileManagementDbContext>, IFileManagementDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */
        public DbSet<File> Files { get; set; }
        public DbSet<FileUser> FileUsers { get; set; }

        public FileManagementDbContext(DbContextOptions<FileManagementDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureFileManagement();
        }
    }
}
