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

public partial class MoveModal
{
    [Inject]
    protected new IStringLocalizer<FileManagementResource> L { get; set; }

    [Inject]
    protected IFileAppService FileAppService { get; set; }

    protected Modal MoveModalRef { get; set; }

    protected MoveFileViewModel MoveEntity { get; set; }

    protected Validations MoveValidationsRef { get; set; }

    protected bool HasMovePermission { get; set; }
    private string MovePolicyName = FileManagementPermissions.File.Move;

    protected virtual Task ClosingMoveModal(ModalClosingEventArgs eventArgs)
    {
        eventArgs.Cancel = eventArgs.CloseReason == CloseReason.FocusLostClosing;
        return Task.CompletedTask;
    }

    protected virtual Task CloseMoveModalAsync()
    {
        MoveEntity = new MoveFileViewModel();
        return InvokeAsync(new Func<Task>(MoveModalRef.Hide));
    }

    protected override async Task OnInitializedAsync()
    {
        MoveEntity = new MoveFileViewModel();

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
        if (MovePolicyName != null)
        {
            HasMovePermission = await AuthorizationService.IsGrantedAsync(MovePolicyName);
        }
    }

    protected virtual async Task MoveEntityAsync()
    {
        try
        {
            var validate = true;
            if (MoveValidationsRef != null)
            {
                validate = await MoveValidationsRef.ValidateAll();
            }

            if (validate)
            {
                await OnMovingEntityAsync();
                await CheckMovePolicyAsync();
                var dto = new MoveFileInput
                {
                    NewFileName = MoveEntity.NewFileName,
                    NewParentId = Guid.Parse(MoveEntity.NewParentId)
                };
                await FileAppService.MoveAsync(MoveEntity.Id, dto);
                await OnMovedEntityAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    public MoveModal()
    {
        LocalizationResource = typeof(FileManagementResource);
    }

    protected virtual async Task OnMovedEntityAsync()
    {
        MoveEntity = new MoveFileViewModel();
        await InvokeAsync(MoveModalRef.Hide);
        await Notify.Success(GetMoveMessage());
        if (Saved != null)
        {
            await Saved.Invoke("Ok");
        }
    }
    protected virtual string GetMoveMessage() => (string)L["MovedSuccessfully"];

    [Parameter]
    public Func<string, Task> Saved { get; set; }

    protected virtual Task OnMovingEntityAsync() => Task.CompletedTask;

    public virtual async Task OpenAsync(Guid id)
    {
        try
        {
            if (MoveValidationsRef != null)
            {
                await MoveValidationsRef.ClearAll();
            }

            var dto = await FileAppService.GetAsync(id);
            await CheckMovePolicyAsync();
            MoveEntity = new MoveFileViewModel() { Id = id, NewFileName = dto.FileName };

            // Mapper will not notify Blazor that binded values are changed
            // so we need to notify it manually by calling StateHasChanged
            await InvokeAsync(async () =>
            {
                StateHasChanged();
                if (MoveModalRef != null)
                {
                    await MoveModalRef.Show();
                }
            });
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task CheckMovePolicyAsync()
    {
        await CheckPolicyAsync(MovePolicyName).ConfigureAwait(false);
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
            MoveModalRef?.Dispose();
        }
        base.Dispose(disposing);
    }
}