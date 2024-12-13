using System.Threading.Tasks;
using Syrna.FileManagement.Localization;
using Syrna.FileManagement.Permissions;
using Volo.Abp.UI.Navigation;

namespace Syrna.FileManagement.Menus;

public class MyMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenu(context);
        }
    }

    private async Task ConfigureMainMenu(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<FileManagementResource>();

        if (await context.IsGrantedAsync(FileManagementPermissions.File.Default))
        {
            context.Menu.AddItem(new ApplicationMenuItem(MyMenus.File,
                l["Menu:File"], "/MyFiles", icon: "fa fa-folder")
            );
        }
    }
}