using Volo.Abp.Reflection;

namespace Syrna.FileManagement.Permissions
{
    public class FileManagementPermissions
    {
        public const string GroupName = "Syrna.FileManagement";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileManagementPermissions));
        }

        public class File
        {
            public const string Default = GroupName + ".File";
            public const string Manage = Default + ".Manage";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
            public const string GetDownloadInfo = Default + ".GetDownloadInfo";
            public const string Move = Default + ".Move";
        }
    }
}
