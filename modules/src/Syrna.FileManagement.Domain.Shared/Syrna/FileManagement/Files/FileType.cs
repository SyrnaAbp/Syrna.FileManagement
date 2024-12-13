using System;

namespace Syrna.FileManagement.Files
{
    [Flags]
    public enum FileType
    {
        Directory = 1,
        RegularFile = 2
    }
}