using System;
using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class FileNameContainsSeparatorException : BusinessException
    {
        public FileNameContainsSeparatorException(string fileName, char separator) : base("FileNameContainsSeparator",
            message: $"The file name ({fileName}) should not contains the separator ({separator}).")
        {
            Data.Add("fileName", fileName);
            Data.Add("separator", separator);
        }
    }
}