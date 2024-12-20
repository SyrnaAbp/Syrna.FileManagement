using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class UnexpectedFileContainerNameException : BusinessException
    {
        public UnexpectedFileContainerNameException(string fileContainerName) : base("UnexpectedFileContainerName",
            message: $"The FileContainerName ({fileContainerName}) is unexpected.")
        {
            Data.Add("fileContainerName", fileContainerName);
        }
        
        public UnexpectedFileContainerNameException(string fileContainerName, string expectedFileContainerName) : base("UnexpectedFileContainerNameWith",
            message: $"The FileContainerName ({fileContainerName}) is unexpected, it should be {expectedFileContainerName}.")
        {
            Data.Add("fileContainerName", fileContainerName);
            Data.Add("expectedFileContainerName", expectedFileContainerName);
        }
    }
}