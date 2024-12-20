using System;
using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class UnexpectedFileTypeException : BusinessException
    {
        public UnexpectedFileTypeException(Guid fileId, FileType fileType) : base("UnexpectedFileTypeException",
            message: $"The type ({fileType}) of the file ({fileId}) is unexpected.")
        {
            Data.Add("fileId", fileId);
            Data.Add("fileType", fileType);
        }

        public UnexpectedFileTypeException(Guid fileId, FileType fileType, FileType expectedFileType) : base("UnexpectedFileTypeExceptionWith",
            message: $"The type ({fileType}) of the file ({fileId}) is unexpected, it should be {expectedFileType}.")
        {
            Data.Add("fileId", fileId);
            Data.Add("fileType", fileType);
            Data.Add("expectedFileType", expectedFileType);
        }
    }
}