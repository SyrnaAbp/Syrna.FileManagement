using System;
using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class FileAlreadyExistsException : BusinessException
    {
        public FileAlreadyExistsException(string filePath) : base("FileAlreadyExists",message: $"The file ({filePath}) already exists")
        {
            Data.Add("filePath", filePath);
        }

        public FileAlreadyExistsException(string fileName, Guid? parentId) : base("FileAlreadyExistsWithParent",message: $"The file (name: {fileName}, parentId: {parentId}) already exists")
        {
            Data.Add("fileName", fileName);
            Data.Add("parentId", parentId);
        }
    }
}