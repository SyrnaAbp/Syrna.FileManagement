﻿using System;
using System.Threading.Tasks;
using Syrna.FileManagement.Files;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

namespace Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

[ViewComponent(Name = "FileManager")]
[Widget(
    ScriptTypes = new[] { typeof(FileManagerScriptBundleContributor) },
    StyleTypes = new[] { typeof(FileManagerStyleBundleContributor) },
    RefreshUrl = "/FileManagementWidgets/FileManager",
    AutoInitialize = true
)]
public class FileManagerWidgetViewComponent : AbpViewComponent
{
    private readonly IFileAppService _fileAppService;

    public FileManagerWidgetViewComponent(IFileAppService fileAppService)
    {
        _fileAppService = fileAppService;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync(string fileContainerName, Guid? ownerUserId,
        Guid? parentId, FileManagerPolicyModel policy = null)
    {
        if (parentId is null)
        {
            return View(
                "~/Pages/FileManagement/Components/FileManagerWidget/Default.cshtml",
                new FileManagerViewModel(fileContainerName, ownerUserId, null, null, null, policy));
        }

        var dir = await _fileAppService.GetAsync(parentId.Value);
        var location = (await _fileAppService.GetLocationAsync(dir.Id)).Location;

        return View(
            "~/Pages/FileManagement/Components/FileManagerWidget/Default.cshtml",
            new FileManagerViewModel(dir.FileContainerName, dir.OwnerUserId, parentId, dir.ParentId, location, policy));
    }
}