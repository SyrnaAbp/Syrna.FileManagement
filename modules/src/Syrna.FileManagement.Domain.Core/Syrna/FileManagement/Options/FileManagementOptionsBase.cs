using Syrna.FileManagement.Containers;
using Syrna.FileManagement.Options.Containers;

namespace Syrna.FileManagement.Options;

public abstract class FileManagementOptionsBase<TConfiguration> where TConfiguration : IFileContainerConfiguration, new()
{
    public FileContainerConfigurations<TConfiguration> Containers { get; } = new();
}