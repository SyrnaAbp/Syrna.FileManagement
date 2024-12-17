using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Syrna.FileManagement.Files.Dtos;
using Syrna.FileManagement.Localization;
using Syrna.FileManagement.Files;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Users;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using Syrna.FileManagement.Permissions;
using Volo.Abp.BlazoriseUI;
using Syrna.FileManagement.Blazor.ViewModels;
using Blazorise;

namespace Syrna.FileManagement.Blazor.Components;

public partial class FileManagerComponent
{
    [Parameter]
    public string FileContainerName { get; set; }

    [Parameter]
    public Guid OwnerUserId { get; set; }

    protected Guid GrandParentId { get; set; }

    protected TextFileContentModal TextFileContentModalRef;
    protected ImageFileContentModal ImageFileContentModalRef;
    protected CreateDirectoryModal CreateDirectoryModalRef;
    protected DetailsModal DetailsModalRef;
    protected MoveModal MoveModalRef;
    protected RenameModal RenameModalRef;
    protected UploadModal UploadModalRef;
    protected DownloadLink DownloadLinkRef;

    protected string CreateDirectoryPolicyName;
    protected string DeleteDirectoryPolicyName;
    protected string MoveDirectoryPolicyName;
    protected string RenameDirectoryPolicyName;

    protected string DeleteFilePolicyName;
    protected string MoveFilePolicyName;
    protected string RenameFilePolicyName;
    protected string DownloadFilePolicyName;
    protected string UploadFilePolicyName;

    protected bool HasCreateDirectoryPermission { get; set; }
    protected bool HasDeleteDirectoryPermission { get; set; }
    protected bool HasMoveDirectoryPermission { get; set; }
    protected bool HasRenameDirectoryPermission { get; set; }

    protected bool HasDeleteFilePermission { get; set; }
    protected bool HasMoveFilePermission { get; set; }
    protected bool HasRenameFilePermission { get; set; }
    protected bool HasDownloadFilePermission { get; set; }
    protected bool HasUploadFilePermission { get; set; }

    protected PageToolbar Toolbar { get; } = new();

    protected List<TableColumn> FileManagerComponentTableColumns => TableColumns.Get<FileManagerComponent>();

    public FileManagerComponent()
    {
        ObjectMapperContext = typeof(FileManagementBlazorModule);
        LocalizationResource = typeof(FileManagementResource);

        CreateDirectoryPolicyName = FileManagementPermissions.File.Create;
        DeleteDirectoryPolicyName = FileManagementPermissions.File.Delete;
        MoveDirectoryPolicyName = FileManagementPermissions.File.Move;
        RenameDirectoryPolicyName = FileManagementPermissions.File.Update;

        DeleteFilePolicyName = FileManagementPermissions.File.Delete;
        MoveFilePolicyName = FileManagementPermissions.File.Move;
        RenameFilePolicyName = FileManagementPermissions.File.Update;
        DownloadFilePolicyName = FileManagementPermissions.File.GetDownloadInfo;
        UploadFilePolicyName = FileManagementPermissions.File.Create;
    }

    protected override async Task OnInitializedAsync()
    {
        FileContainerName = "default";
        OwnerUserId = CurrentUser.GetId();
        //var root = await AppService.GetByPathAsync("/", FileContainerName, OwnerUserId);
        //GrandParentId = root.FileInfo.Id;
        await base.OnInitializedAsync();
    }

    protected override async Task SetPermissionsAsync()
    {
        if (CreateDirectoryPolicyName != null)
        {
            HasCreateDirectoryPermission = await AuthorizationService.IsGrantedAsync(CreateDirectoryPolicyName);
        }
        if (DeleteDirectoryPolicyName != null)
        {
            HasDeleteDirectoryPermission = await AuthorizationService.IsGrantedAsync(DeleteDirectoryPolicyName);
        }
        if (MoveDirectoryPolicyName != null)
        {
            HasMoveDirectoryPermission = await AuthorizationService.IsGrantedAsync(MoveDirectoryPolicyName);
        }
        if (RenameDirectoryPolicyName != null)
        {
            HasRenameDirectoryPermission = await AuthorizationService.IsGrantedAsync(RenameDirectoryPolicyName);
        }

        if (DeleteFilePolicyName != null)
        {
            HasDeleteFilePermission = await AuthorizationService.IsGrantedAsync(DeleteFilePolicyName);
        }
        if (MoveFilePolicyName != null)
        {
            HasMoveFilePermission = await AuthorizationService.IsGrantedAsync(MoveFilePolicyName);
        }
        if (RenameFilePolicyName != null)
        {
            HasRenameFilePermission = await AuthorizationService.IsGrantedAsync(RenameFilePolicyName);
        }
        if (DownloadFilePolicyName != null)
        {
            HasDownloadFilePermission = await AuthorizationService.IsGrantedAsync(DownloadFilePolicyName);
        }
        if (UploadFilePolicyName != null)
        {
            HasUploadFilePermission = await AuthorizationService.IsGrantedAsync(UploadFilePolicyName);
        }
        await base.SetPermissionsAsync();
    }

    public Guid? CurrentParentId { get; set; }

    protected override Task UpdateGetListInputAsync()
    {
        GetListInput.FileContainerName = FileContainerName;
        GetListInput.OwnerUserId = OwnerUserId;
        GetListInput.ParentId = CurrentParentId;
        return base.UpdateGetListInputAsync();
    }

    public async Task ParentIdChanged(Guid? parentId)
    {
        CurrentParentId = parentId;
        await GetEntitiesAsync();
        BreadcrumbItems.Clear();
        await SetBreadcrumbItemsAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected override async ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Location"].Value));
        _menuItemParents.Clear();
        await FindParents(CurrentParentId);
        _menuItemParents.Reverse();
        BreadcrumbItems.AddRange(_menuItemParents);
        await base.SetBreadcrumbItemsAsync();
    }
    private readonly List<Volo.Abp.BlazoriseUI.BreadcrumbItem> _menuItemParents = [];
    public async Task FindParents(Guid? id)
    {
        if (id != null)
        {
            var item = await AppService.GetAsync(id.Value);
            if (item != null)
            {
                _menuItemParents.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(item.FileName));
                await FindParents(item.ParentId);
            }
        }
        await Task.CompletedTask;
    }
    protected async Task CreateDirectory()
    {
        await CreateDirectoryModalRef.OpenAsync(FileContainerName, OwnerUserId, CurrentParentId);
    }
    protected async Task UploadFile()
    {
        await UploadModalRef.OpenAsync(FileContainerName, OwnerUserId, CurrentParentId);
    }
    protected async Task AfterSavedOrUpdated(string status)
    {
        await GetEntitiesAsync();
        await InvokeAsync(StateHasChanged);
    }

    public async Task GotoParentId()
    {
        if (CurrentParentId != null)
        {
            var item = await AppService.GetAsync(CurrentParentId.Value);
            await ParentIdChanged(item.ParentId);
        }
        await Task.CompletedTask;
    }

    protected override ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton("..", GotoParentId, "fas fa-circle-left");

        Toolbar.AddButton(@L["CreateDirectory"], CreateDirectory, "fas fa-folder-plus");

        Toolbar.AddButton(@L["UploadFile"], UploadFile, "fas fa-upload");

        return base.SetToolbarItemsAsync();
    }

    protected override ValueTask SetEntityActionsAsync()
    {
        EntityActions
            .Get<FileManagerComponent>()
            .AddRange(new EntityAction[]
            {
                    new EntityAction
                    {
                        Text = L["Download"],
                        Visible = (data) => data.As<FileInfoViewModel>().FileType==FileType.RegularFile?HasDownloadFilePermission:false,
                        Clicked = async (data) =>
                        {
                            var row=data.As<FileInfoViewModel>();
                            var info=await AppService.GetDownloadInfoAsync(row.Id);
                            await DownloadLinkRef.Setup(info.DownloadUrl,info.ExpectedFileName);
                            await DownloadLinkRef.Click();
                        }
                    },
                    new EntityAction
                    {
                        Text = L["Rename"],
                        Visible = (data) => data.As<FileInfoViewModel>().FileType==FileType.RegularFile?HasRenameFilePermission:HasRenameDirectoryPermission,
                        Clicked = async (data) =>
                        {
                            var row=data.As<FileInfoViewModel>();
                            await RenameModalRef.OpenAsync(row.Id);
                        }
                    },
                    new EntityAction
                    {
                        Text = L["Move"],
                        Visible = (data) => data.As<FileInfoViewModel>().FileType==FileType.RegularFile?HasMoveFilePermission:HasMoveDirectoryPermission,
                        Clicked = async (data) =>
                        {
                            var row=data.As<FileInfoViewModel>();
                            await MoveModalRef.OpenAsync(row.Id);
                        }
                    },
                    new EntityAction
                    {
                        Text = L["Delete"],
                        Visible = (data) => data.As<FileInfoViewModel>().FileType==FileType.RegularFile?HasDeleteFilePermission:HasDeleteDirectoryPermission,
                        Clicked = async (data) => await DeleteAsync(data.As<FileInfoViewModel>().Id),
                        ConfirmationMessage = (data) => GetDeleteConfirmationMessage(data.As<FileInfoViewModel>())
                    },
                    new EntityAction
                    {
                        Text = L["Details"],
                        Visible = (data) => data.As<FileInfoViewModel>().Id != GrandParentId,
                        Clicked = async (data) =>
                        {
                            var row=data.As<FileInfoViewModel>();
                            await DetailsModalRef.OpenDetailsModalAsync(row.Id);
                        }
                   },
                    new EntityAction
                    {
                        Text = L["FileContent"],
                        Visible = (data) => data.As<FileInfoViewModel>().FileType==FileType.RegularFile,
                        Clicked = async (data) =>
                        {
                            var row=data.As<FileInfoViewModel>();
                            if (SharedFunctions.IsTextFile(row.MimeType))
                            await TextFileContentModalRef.OpenAsync(row.Id);
                            else if (SharedFunctions.IsImageFile(row.MimeType))
                            await ImageFileContentModalRef.OpenAsync(row.Id);
                            else
                            {
                                throw new UserFriendlyException("Unsupported mime type");
                            }
                        }
                    }
            });

        return base.SetEntityActionsAsync();
    }

    private async Task DeleteAsync(Guid id)
    {
        try
        {
            await CheckDeletePolicyAsync();
            await OnDeletingEntityAsync();
            await AppService.DeleteAsync(id);
            await OnDeletedEntityAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    protected virtual async Task CheckDeletePolicyAsync()
    {
        await CheckPolicyAsync(DeleteFilePolicyName);
    }

    protected override async Task GetEntitiesAsync()
    {
        try
        {
            await UpdateGetListInputAsync();
            var result = await AppService.GetListAsync(GetListInput);
            Entities = MapToListViewModel(result.Items);
            foreach (var item in Entities) item.ParentIdChanged = ParentIdChanged;
            TotalCount = (int?)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    private IReadOnlyList<FileInfoViewModel> MapToListViewModel(IReadOnlyList<FileInfoDto> dtos)
    {
        return ObjectMapper.Map<IReadOnlyList<FileInfoDto>, List<FileInfoViewModel>>(dtos);
    }

    private async Task OnDeletedEntityAsync()
    {
        await GetEntitiesAsync();
        await InvokeAsync(StateHasChanged);
        await Notify.Success(GetDeleteMessage());
    }
    protected virtual string GetDeleteMessage() => L["DeletedSuccessfully"];

    protected virtual Task OnDeletingEntityAsync() => Task.CompletedTask;

    protected virtual string GetDeleteConfirmationMessage(FileInfoViewModel entity)
    {
        return string.Format(L["FileDeletionConfirmationMessage"], entity.Id);
    }

    protected override async ValueTask SetTableColumnsAsync()
    {
        FileManagerComponentTableColumns
            .AddRange(new TableColumn[]
            {
                    new TableColumn
                    {
                        Title = L["Actions"],
                        Actions = EntityActions.Get<FileManagerComponent>(),
                    },
                    new TableColumn
                    {
                        Title = L["FileFileName"],
                        Sortable = true,
                        Data = nameof(FileInfoViewModel.FileName),
                        ValueConverter=FormatFileName,
                        Component=typeof(FilePathComponent)
                    },
                    new TableColumn
                    {
                        Title = L["FileByteSize"],
                        Sortable = true,
                        Data = nameof(FileInfoViewModel.ByteSize),
                        ValueConverter=FormatByteSize
                    },
                    new TableColumn
                    {
                        Title = L["Modified"],
                        Sortable = true,
                        Data = nameof(FileInfoViewModel.LastModificationTime),
                        ValueConverter=FormatModifiedTime
                    },
            });

        //FileManagerComponentTableColumns.AddRange(await GetExtensionTableColumnsAsync(IdentityModuleExtensionConsts.ModuleName,
        //    IdentityModuleExtensionConsts.EntityNames.Role));

        await base.SetTableColumnsAsync();
    }

    private static string FormatFileName(object data)
    {
        var row = data.As<FileInfoViewModel>();
        var cellData = row.FileName;
        if (row.FileType == FileType.Directory)
        {
            return $"<a class='dir-link' href='#' data-dir-id='${row.Id}'><i class='directory-icon fa fa-folder fa-fw' aria-hidden='true'></i>${cellData}</a>";
        }
        return $"<i class='file-icon fa fa-file fa-fw' aria-hidden='true'></i>${cellData}";
    }

    private static string FormatModifiedTime(object data)
    {
        var row = data.As<FileInfoViewModel>();

        return row.LastModificationTime == null ? row.CreationTime.ToString("F") : row.LastModificationTime.Value.ToString("F");
    }

    private static string FormatByteSize(object data)
    {
        var row = data.As<FileInfoViewModel>();
        var bytes = row.ByteSize;

        return SharedFunctions.HumanFileSize(bytes, true);
    }
}
