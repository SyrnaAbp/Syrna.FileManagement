using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Syrna.FileManagement.Users;

public class FileUserLookupService : UserLookupService<FileUser, IFileUserRepository>, IFileUserLookupService
{
    public FileUserLookupService(
        IFileUserRepository userRepository,
        IUnitOfWorkManager unitOfWorkManager)
        : base(
            userRepository,
            unitOfWorkManager)
    {
            
    }

    protected override FileUser CreateUser(IUserData externalUser)
    {
        return new FileUser(externalUser);
    }
}