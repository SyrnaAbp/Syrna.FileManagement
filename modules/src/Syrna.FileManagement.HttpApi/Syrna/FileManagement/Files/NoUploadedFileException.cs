using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class NoUploadedFileException : BusinessException
    {
        public NoUploadedFileException() : base("NoUploadedFile")
        {
            
        }
    }
}