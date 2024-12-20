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

public partial class RenameModal
{
    [Inject]
    protected new IStringLocalizer<FileManagementResource> L { get; set; }

    [Inject]
    protected IFileAppService FileAppService { get; set; }

    protected Modal RenameModalRef { get; set; }

    protected RenameFileViewModel RenameEntity { get; set; }

    protected Validations RenameValidationsRef { get; set; }

    protected bool HasRenamePermission { get; set; }
    private string RenamePolicyName = FileManagementPermissions.File.Update;

    protected virtual Task ClosingRenameModal(ModalClosingEventArgs eventArgs)
    {
        eventArgs.Cancel = eventArgs.CloseReason == CloseReason.FocusLostClosing;
        return Task.CompletedTask;
    }

    protected virtual Task CloseRenameModalAsync()
    {
        RenameEntity = new RenameFileViewModel();
        return InvokeAsync(new Func<Task>(RenameModalRef.Hide));
    }

    protected override async Task OnInitializedAsync()
    {
        RenameEntity = new RenameFileViewModel();

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
        if (RenamePolicyName != null)
        {
            HasRenamePermission = await AuthorizationService.IsGrantedAsync(RenamePolicyName);
        }
    }

    protected virtual async Task RenameEntityAsync()
    {
        try
        {
            var validate = true;
            if (RenameValidationsRef != null)
            {
                validate = await RenameValidationsRef.ValidateAll();
            }

            if (validate)
            {
                await OnRenamingEntityAsync();
                await CheckRenamePolicyAsync();
                var dto = new UpdateFileInfoInput
                {
                    FileName = RenameEntity.FileName,
                };
                await FileAppService.UpdateInfoAsync(RenameEntity.Id, dto);
                await OnRenamedEntityAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    public RenameModal()
    {
        LocalizationResource = typeof(FileManagementResource);
    }

    protected virtual async Task OnRenamedEntityAsync()
    {
        RenameEntity = new RenameFileViewModel();
        await InvokeAsync(RenameModalRef.Hide);
        await Notify.Success(GetRenameMessage());
        if (Saved != null)
        {
            await Saved.Invoke("Ok");
        }
    }
    protected virtual string GetRenameMessage() => L["SuccessfullyRenamed"];

    [Parameter]
    public Func<string, Task> Saved { get; set; }

    protected virtual Task OnRenamingEntityAsync() => Task.CompletedTask;

    public virtual async Task OpenAsync(Guid id)
    {
        try
        {
            if (RenameValidationsRef != null)
            {
                await RenameValidationsRef.ClearAll();
            }

            var dto = await FileAppService.GetAsync(id);
            RenameEntity = ObjectMapper.Map<FileInfoDto, RenameFileViewModel>(dto);
            await CheckRenamePolicyAsync();

            // Mapper will not notify Blazor that binded values are changed
            // so we need to notify it manually by calling StateHasChanged
            await InvokeAsync(async () =>
            {
                StateHasChanged();
                if (RenameModalRef != null)
                {
                    await RenameModalRef.Show();
                }
            });
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task CheckRenamePolicyAsync()
    {
        await CheckPolicyAsync(RenamePolicyName).ConfigureAwait(false);
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
            RenameModalRef?.Dispose();
        }
        base.Dispose(disposing);
    }
}