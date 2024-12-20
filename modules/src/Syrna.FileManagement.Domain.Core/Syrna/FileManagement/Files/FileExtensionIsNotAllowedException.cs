using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class FileExtensionIsNotAllowedException : BusinessException
    {
        public FileExtensionIsNotAllowedException(string fileName) : base("FileExtensionIsNotAllowed",
            "FileExtensionIsNotAllowed",
            $"The extension of {fileName} is not allowed.")
        {
            Data.Add("fileName", fileName);
        }
    }
}