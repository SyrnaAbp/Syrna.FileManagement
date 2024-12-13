using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class FileExtensionIsNotAllowedException : BusinessException
    {
        public FileExtensionIsNotAllowedException(string fileName) : base(
            "FileExtensionIsNotAllowed",
            $"The extension of {fileName} is not allowed.")
        {
        }
    }
}