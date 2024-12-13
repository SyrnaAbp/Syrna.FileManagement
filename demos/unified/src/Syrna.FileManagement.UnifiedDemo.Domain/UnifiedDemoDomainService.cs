using Volo.Abp.Domain.Services;
using Volo.Abp.Users;

namespace Syrna.FileManagement.UnifiedDemo
{
    public abstract class UnifiedDemoDomainService : DomainService
    {
        protected ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();
    }
}