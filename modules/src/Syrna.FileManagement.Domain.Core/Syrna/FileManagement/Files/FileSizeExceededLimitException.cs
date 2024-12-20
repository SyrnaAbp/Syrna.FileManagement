﻿using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class FileSizeExceededLimitException : BusinessException
    {
        public FileSizeExceededLimitException(string fileName, long fileByteSize, long maxByteSize) : base(
            "FileSizeExceededLimit",
            $"The size of the file (name: {fileName}, size: {fileByteSize}) exceeded the limit: {maxByteSize}.")
        {
            Data.Add("fileName", fileName);
            Data.Add("fileByteSize", fileByteSize);
            Data.Add("maxByteSize", maxByteSize);
        }
    }
}