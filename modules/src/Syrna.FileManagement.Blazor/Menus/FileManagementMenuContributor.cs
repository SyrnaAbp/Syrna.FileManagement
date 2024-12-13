using System.Threading.Tasks;
using Syrna.FileManagement.Localization;
using Syrna.FileManagement.Permissions;
using Volo.Abp.UI.Navigation;

namespace Syrna.FileManagement.Blazor.Menus
{
    public class FileManagementMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<FileManagementResource>();

            if (await context.IsGrantedAsync(FileManagementPermissions.File.Default))
            {
                context.Menu.AddItem(new ApplicationMenuItem(FileManagementMenus.Prefix,
                    l["Menu:File"], "/FileManagement/Files/File", icon: "fa fa-folder")
                );
            }
        }
    }
}