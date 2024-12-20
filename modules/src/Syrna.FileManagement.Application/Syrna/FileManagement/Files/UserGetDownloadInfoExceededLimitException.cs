﻿using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class UserGetDownloadInfoExceededLimitException : BusinessException
    {
        public UserGetDownloadInfoExceededLimitException() : base("UserGetDownloadInfoExceededLimit",
            "The number of times you get download information exceeds the limit.")
        {
        }
    }
}