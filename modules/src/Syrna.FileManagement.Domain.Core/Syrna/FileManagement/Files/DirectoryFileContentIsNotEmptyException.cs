﻿using System;
using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class DirectoryFileContentIsNotEmptyException : BusinessException
    {
        public DirectoryFileContentIsNotEmptyException() : base("DirectoryFileContentIsNotEmpty",message: "Content should be empty if the file is a directory.")
        {
        }
    }
}