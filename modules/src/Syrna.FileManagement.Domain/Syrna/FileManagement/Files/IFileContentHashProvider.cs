namespace Syrna.FileManagement.Files
{
    public interface IFileContentHashProvider
    {
        string GetHashString(byte[] fileContent);
    }
}