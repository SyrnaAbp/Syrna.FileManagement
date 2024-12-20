using System;
using System.Globalization;
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

public partial class DetailsModal
{
    [Inject]
    protected new IStringLocalizer<FileManagementResource> L { get; set; }

    [Inject]
    protected IFileAppService FileInfoAppService { get; set; }

    protected Modal DetailsModalRef { get; set; }

    protected FileDetailViewModel DetailsEntity { get; set; }

    protected bool IsSystemUserMessage { get; set; }

    protected virtual Task ClosingDetailsModal(ModalClosingEventArgs eventArgs)
    {
        eventArgs.Cancel = eventArgs.CloseReason == CloseReason.FocusLostClosing;
        return Task.CompletedTask;
    }

    public DetailsModal()
    {
        LocalizationResource = typeof(FileManagementResource);
    }

    protected override async Task OnInitializedAsync()
    {
        DetailsEntity = new FileDetailViewModel();

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
        if (DetailsPolicyName != null)
        {
            HasDetailsPermission = await AuthorizationService.IsGrantedAsync(DetailsPolicyName);
        }
    }

    protected bool HasDetailsPermission { get; set; }
    private string DetailsPolicyName = FileManagementPermissions.File.Default;
    public virtual async Task OpenDetailsModalAsync(Guid id)
    {
        try
        {
            await CheckDetailsPolicyAsync();

            var entityDto = await FileInfoAppService.GetAsync(id);

            DetailsEntity = await MapToDetailsEntity(entityDto);

            await InvokeAsync(async () =>
            {
                StateHasChanged();
                if (DetailsModalRef != null)
                {
                    await DetailsModalRef.Show();
                }
            });
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task<FileDetailViewModel> MapToDetailsEntity(FileInfoDto dto)
    {
        return new FileDetailViewModel
        {
            Id = dto.Id.ToString(),
            FileName = dto.FileName,
            FileType = dto.FileType,
            MimeType = dto.MimeType,
            ByteSize = SharedFunctions.HumanFileSize(dto.ByteSize),
            Hash = dto.Hash,
            Location = (await FileInfoAppService.GetLocationAsync(dto.Id)).Location.FilePath,
            Creator = dto.Creator?.UserName,
            Created = ToDateTimeString(dto.CreationTime),
            LastModifier = dto.LastModifier?.UserName,
            Modified = ToDateTimeString(dto.LastModificationTime ?? dto.CreationTime)
        };
    }

    protected virtual string ToDateTimeString(DateTime time)
    {
        return time.ToString(CultureInfo.CurrentUICulture);
    }

    protected virtual async Task CheckDetailsPolicyAsync()
    {
        await CheckPolicyAsync(DetailsPolicyName).ConfigureAwait(false);
    }

    protected virtual async Task CheckPolicyAsync(string policyName)
    {
        if (string.IsNullOrEmpty(policyName))
            return;
        await AuthorizationService.CheckAsync(policyName).ConfigureAwait(false);
    }

    protected virtual Task CloseDetailsModalAsync()
    {
        return InvokeAsync(new Func<Task>(DetailsModalRef.Hide));
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DetailsModalRef?.Dispose();
        }
        base.Dispose(disposing);
    }
}