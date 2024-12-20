using System;
using System.Threading.Tasks;
using Syrna.FileManagement.Files.Dtos;

namespace Syrna.FileManagement.Files
{
    public class FileContentAppService : FileManagementAppService, IFileContentAppService
    {
        private readonly IFileRepository _repository;
        private readonly IFileManager _fileManager;
        public virtual async Task<FileDownloadOutput> GetContentAsync(Guid id)
        {
            var provider = LazyServiceProvider.LazyGetRequiredService<ILocalFileDownloadProvider>();

            var file = await GetEntityByIdAsync(id);

            return new FileDownloadOutput
            {
                FileName = file.FileName,
                MimeType = file.MimeType,
                Content = await provider.GetDownloadBytesAsync(file)
            };
        }

        public FileContentAppService(
            IFileManager fileManager,
            IFileRepository repository
            )
        {
            _fileManager = fileManager;
            _repository = repository;
        }

        protected async Task<File> GetEntityByIdAsync(Guid id)
        {
            return await _repository.GetAsync(id);
        }
    }
}