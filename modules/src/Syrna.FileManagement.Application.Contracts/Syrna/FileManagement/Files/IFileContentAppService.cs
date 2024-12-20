using System;
using System.Threading.Tasks;
using Syrna.FileManagement.Files.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Syrna.FileManagement.Files
{
    public interface IFileContentAppService : IApplicationService, IRemoteService
    {
        Task<FileDownloadOutput> GetContentAsync(Guid id);
    }
}