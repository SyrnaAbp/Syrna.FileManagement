using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class LocalFileDownloadInvalidTokenException : BusinessException
    {
        public LocalFileDownloadInvalidTokenException() : base("LocalFileDownloadInvalidToken",
            "The file download token is not invalid.")
        {
        }
    }
}