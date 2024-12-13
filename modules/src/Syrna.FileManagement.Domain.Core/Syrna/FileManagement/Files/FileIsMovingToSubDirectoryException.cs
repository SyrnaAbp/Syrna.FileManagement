using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class FileIsMovingToSubDirectoryException : BusinessException
    {
        public FileIsMovingToSubDirectoryException() : base(
            message: "A directory cannot be moved from a directory to one of its sub directories.")
        {
        }
    }
}