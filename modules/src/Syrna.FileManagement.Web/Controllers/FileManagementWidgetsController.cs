using System;
using System.Threading.Tasks;
using Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;
using Microsoft.AspNetCore.Mvc;

namespace Syrna.FileManagement.Web.Controllers;

public class FileManagementWidgetsController : FileManagementControllerBase
{
    public virtual Task<IActionResult> FileManager(string fileContainerName, Guid? ownerUserId, Guid? parentId)
    {
        return Task.FromResult((IActionResult)ViewComponent(typeof(FileManagerWidgetViewComponent),
            new { fileContainerName, ownerUserId, parentId }));
    }
}