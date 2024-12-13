using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class UnexpectedFileContainerNameException : BusinessException
    {
        public UnexpectedFileContainerNameException(string fileContainerName) : base(
            message: $"The FileContainerName ({fileContainerName}) is unexpected.")
        {
        }
        
        public UnexpectedFileContainerNameException(string fileContainerName, string expectedFileContainerName) : base(
            message: $"The FileContainerName ({fileContainerName}) is unexpected, it should be {expectedFileContainerName}.")
        {
        }
    }
}