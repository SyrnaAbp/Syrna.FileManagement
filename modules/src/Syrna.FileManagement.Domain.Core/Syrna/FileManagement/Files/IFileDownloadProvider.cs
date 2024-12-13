using System.Threading.Tasks;

namespace Syrna.FileManagement.Files
{
    public interface IFileDownloadProvider
    {
        Task<FileDownloadInfoModel> CreateDownloadInfoAsync(File file);
    }
}