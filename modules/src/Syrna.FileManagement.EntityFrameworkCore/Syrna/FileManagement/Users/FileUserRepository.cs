using Syrna.FileManagement.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users.EntityFrameworkCore;

namespace Syrna.FileManagement.Users;

public class FileUserRepository : EfCoreUserRepositoryBase<IFileManagementDbContext, FileUser>, IFileUserRepository
{
    public FileUserRepository(IDbContextProvider<IFileManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}