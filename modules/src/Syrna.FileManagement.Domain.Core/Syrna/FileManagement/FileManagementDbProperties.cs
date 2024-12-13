namespace Syrna.FileManagement
{
    public static class FileManagementDbProperties
    {
        public static string DbTablePrefix { get; set; } = "SyrnaFileManagement";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "SyrnaFileManagement";
    }
}
