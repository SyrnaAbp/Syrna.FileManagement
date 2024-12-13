using Syrna.FileManagement.Containers;

namespace Syrna.FileManagement.Options.Containers;

public interface IFileContainerConfigurationProvider
{
    TConfiguration Get<TConfiguration>(string fileContainerName) where TConfiguration : IFileContainerConfiguration;

    IFileContainerConfiguration Get(string fileContainerName);
}