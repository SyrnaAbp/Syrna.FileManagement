using System;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Syrna.FileManagement.Blazor.ViewModels;
using Syrna.FileManagement.Files;
using Syrna.FileManagement.Files.Dtos;
using Syrna.FileManagement.Localization;
using Syrna.FileManagement.Permissions;

namespace Syrna.FileManagement.Blazor.Components;

public partial class CreateDirectoryModal
{
    [Inject]
    protected new IStringLocalizer<FileManagementResource> L { get; set; }

    [Inject]
    protected IFileAppService FileAppService { get; set; }

    protected Modal CreateModalRef { get; set; }

    protected CreateDirectoryViewModel NewEntity { get; set; }

    protected Validations CreateValidationsRef { get; set; }

    protected bool HasCreatePermission { get; set; }
    private string CreatePolicyName = FileManagementPermissions.File.Create;

    protected virtual Task ClosingCreateModal(ModalClosingEventArgs eventArgs)
    {
        eventArgs.Cancel = eventArgs.CloseReason == CloseReason.FocusLostClosing;
        return Task.CompletedTask;
    }

    protected virtual Task CloseCreateModalAsync()
    {
        NewEntity = new CreateDirectoryViewModel();
        return InvokeAsync(new Func<Task>(CreateModalRef.Hide));
    }

    protected override async Task OnInitializedAsync()
    {
        NewEntity = new CreateDirectoryViewModel();

        await TrySetPermissionsAsync().ConfigureAwait(false);
        await base.OnInitializedAsync();
    }

    private async Task TrySetPermissionsAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        await SetPermissionsAsync();
    }

    protected virtual async Task SetPermissionsAsync()
    {
        if (CreatePolicyName != null)
        {
            HasCreatePermission = await AuthorizationService.IsGrantedAsync(CreatePolicyName);
        }
    }

    protected virtual async Task CreateEntityAsync()
    {
        try
        {
            var validate = true;
            if (CreateValidationsRef != null)
            {
                validate = await CreateValidationsRef.ValidateAll();
            }

            if (validate)
            {
                await OnCreatingEntityAsync();
                await CheckCreatePolicyAsync();
                var dto = new CreateFileInput
                {
                    FileContainerName = NewEntity.FileContainerName,
                    OwnerUserId = NewEntity.OwnerUserId,
                    FileName = NewEntity.DirectoryName,
                    FileType = FileType.Directory,
                    MimeType = null,
                    ParentId = NewEntity.ParentId,
                    Content = null
                };
                await FileAppService.CreateAsync(dto);
                await OnCreatedEntityAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    public CreateDirectoryModal()
    {
        LocalizationResource = typeof(FileManagementResource);
    }

    protected virtual async Task OnCreatedEntityAsync()
    {
        NewEntity = new CreateDirectoryViewModel();
        await InvokeAsync(CreateModalRef.Hide);
        await Notify.Success(GetCreateMessage());
        if (Saved != null)
        {
            await Saved.Invoke("Ok");
        }
    }
    protected virtual string GetCreateMessage() => L["FolderSuccessfullyCreated"];

    [Parameter]
    public Func<string, Task> Saved { get; set; }

    protected virtual Task OnCreatingEntityAsync() => Task.CompletedTask;

    public virtual async Task OpenAsync(string fileContainerName, Guid? ownerUserId, Guid? parentId)
    {
        try
        {
            if (CreateValidationsRef != null)
            {
                await CreateValidationsRef.ClearAll();
            }

            await CheckCreatePolicyAsync();
            NewEntity = new CreateDirectoryViewModel() { FileContainerName = fileContainerName, OwnerUserId = ownerUserId, ParentId = parentId };

            // Mapper will not notify Blazor that binded values are changed
            // so we need to notify it manually by calling StateHasChanged
            await InvokeAsync(async () =>
            {
                StateHasChanged();
                if (CreateModalRef != null)
                {
                    await CreateModalRef.Show();
                }
            });
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task CheckCreatePolicyAsync()
    {
        await CheckPolicyAsync(CreatePolicyName).ConfigureAwait(false);
    }

    protected virtual async Task CheckPolicyAsync(string policyName)
    {
        if (string.IsNullOrEmpty(policyName))
            return;
        await AuthorizationService.CheckAsync(policyName).ConfigureAwait(false);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CreateModalRef?.Dispose();
        }
        base.Dispose(disposing);
    }
}