using System;
using Syrna.FileManagement.Options.Containers;

namespace Syrna.FileManagement.Options;

public class FileManagementOptions : FileManagementOptionsBase<FileContainerConfiguration>
{
    public Type DefaultFileDownloadProviderType { get; set; }
}