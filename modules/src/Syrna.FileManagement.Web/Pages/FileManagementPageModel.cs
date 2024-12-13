using Syrna.FileManagement.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Syrna.FileManagement.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class FileManagementPageModel : AbpPageModel
    {
        protected FileManagementPageModel()
        {
            LocalizationResourceType = typeof(FileManagementResource);
            ObjectMapperContext = typeof(FileManagementWebModule);
        }
    }
}