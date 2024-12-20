using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Syrna.FileManagement.Blazor.ViewModels;
using Syrna.FileManagement.Containers;
using Syrna.FileManagement.Files;
using Syrna.FileManagement.Files.Dtos;
using Syrna.FileManagement.Localization;
using Syrna.FileManagement.Permissions;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.ExceptionHandling;
using Volo.Abp.Content;

namespace Syrna.FileManagement.Blazor.Components;
public class FileItem
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public byte[] Content { get; set; }
    public string ContentType { get; set; }
    public int FileSize { get; set; }
}
public partial class UploadModal
{
    [Inject]
    protected new IStringLocalizer<FileManagementResource> L { get; set; }

    [Inject]
    protected IFileAppService FileAppService { get; set; }

    protected Modal UploadModalRef { get; set; }

    protected CreateDirectoryViewModel UploadEntity { get; set; }

    protected Validations UploadValidationsRef { get; set; }

    protected bool HasUploadPermission { get; set; }
    private string UploadPolicyName = FileManagementPermissions.File.Create;

    protected virtual Task UploadingCreateModal(ModalClosingEventArgs eventArgs)
    {
        eventArgs.Cancel = eventArgs.CloseReason == CloseReason.FocusLostClosing;
        return Task.CompletedTask;
    }

    protected virtual Task CloseUploadModalAsync()
    {
        UploadEntity = new CreateDirectoryViewModel();
        return InvokeAsync(new Func<Task>(UploadModalRef.Hide));
    }

    public PublicFileContainerConfiguration Configuration { get; set; }
    protected override async Task OnInitializedAsync()
    {
        UploadEntity = new CreateDirectoryViewModel();
        FileItems = new List<FileItem>();

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
        if (UploadPolicyName != null)
        {
            HasUploadPermission = await AuthorizationService.IsGrantedAsync(UploadPolicyName);
        }
    }

    protected virtual async Task UploadEntityAsync()
    {
        try
        {
            var validate = true;
            if (UploadValidationsRef != null)
            {
                validate = await UploadValidationsRef.ValidateAll();
            }

            if (validate)
            {
                await OnUploadingEntityAsync();
                await CheckUploadPolicyAsync();

                var dto = new CreateManyFileWithStreamInput
                {
                    FileContainerName = UploadEntity.FileContainerName,
                    OwnerUserId = UploadEntity.OwnerUserId,
                    ParentId = UploadEntity.ParentId,
                };
                foreach (var uploadedFile in FileItems)
                {
                    dto.FileContents.Add(new RemoteStreamContent(
                        stream: new MemoryStream(uploadedFile.Content),
                        fileName: uploadedFile.FileName,
                        contentType: uploadedFile.ContentType));
                }

                await FileAppService.CreateManyWithStreamAsync(dto);

                await OnUploadedEntityAsync();
            }
        }
        catch (BusinessException ex)
        {
            await HandleBusinessException(ex);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task HandleBusinessException(BusinessException ex)
    {
        if (ex.Code.IsNullOrEmpty())
        {
            await HandleErrorAsync(ex);
        }
        else
        {
            var errorMessage = L[ex.Code].Value;
            if (errorMessage.IsNullOrEmpty())
            {
                await HandleErrorAsync(ex);
            }
            else
            {
                if (ex.Data != null)
                {
                    foreach (var item in ex.Data.Keys)
                    {
                        errorMessage = errorMessage.Replace("{" + item + "}", ex.Data[item].ToString());
                    }
                    await Message.Error(errorMessage);
                }
                else
                {
                    await HandleErrorAsync(ex);
                }
            }
        }

    }

    protected IReadOnlyList<FileItem> FileList = Array.Empty<FileItem>();
    protected List<FileItem> FileItems { get; set; }
    protected DataGrid<FileItem> DataGridRef { get; set; }
    async Task OnChanged(FileChangedEventArgs e)
    {
        try
        {
            foreach (var file in e.Files)
            {
                if (file == null)
                {
                    continue;
                }
                using (MemoryStream result = new MemoryStream())
                {
                    await file.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    var bytes = await result.GetAllBytesAsync();
                    FileItems.Add(new FileItem { Id = Guid.NewGuid(), FileName = file.Name, Content = bytes, ContentType = file.Type, FileSize = bytes.Length });
                }
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.Message);
        }
        finally
        {
            FileList = new List<FileItem>(FileItems);
            //await DataGridRef.Refresh();
            StateHasChanged();
        }
    }

    protected int TotalFiles => FileItems.Count;

    protected int? FilePercentValue { get; set; } = null;

    protected void OnWritten(FileWrittenEventArgs e)
    {
        Console.WriteLine($"File: {e.File.Name} Position: {e.Position} Data: {Convert.ToBase64String(e.Data)}");
        //FileItems.Add(new FileItem { FileName = e.File.Name, Content = e.Data, ContentType = e.File.Type });
    }

    protected void OnProgressed(FileProgressedEventArgs e)
    {
        Console.WriteLine($"File: {e.File.Name} Progress: {e.Percentage}");
        FilePercentValue = (int)e.Percentage;
        if (FilePercentValue == 100) FilePercentValue = null;
    }

    public UploadModal()
    {
        LocalizationResource = typeof(FileManagementResource);
    }

    protected virtual async Task OnUploadedEntityAsync()
    {
        UploadEntity = new CreateDirectoryViewModel();
        await InvokeAsync(UploadModalRef.Hide);
        await Notify.Success(GetUploadMessage());
        if (Saved != null)
        {
            await Saved.Invoke("Ok");
        }
    }

    protected virtual string GetUploadMessage() => L["SuccessfullyUploaded"];

    protected virtual async Task RemoveFileItem(Guid id)
    {
        var item = FileItems.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            FileItems.Remove(item);
            FileList = new List<FileItem>(FileItems);
        }
        await InvokeAsync(StateHasChanged);
    }

    [Parameter]
    public Func<string, Task> Saved { get; set; }

    protected virtual Task OnUploadingEntityAsync() => Task.CompletedTask;

    public virtual async Task OpenAsync(string fileContainerName, Guid? ownerUserId, Guid? parentId)
    {
        try
        {
            if (UploadValidationsRef != null)
            {
                await UploadValidationsRef.ClearAll();
            }

            await CheckUploadPolicyAsync();
            UploadEntity = new CreateDirectoryViewModel() { FileContainerName = fileContainerName, OwnerUserId = ownerUserId, ParentId = parentId };
            Configuration = await FileAppService.GetConfigurationAsync(fileContainerName, ownerUserId);
            AllowedFileExtensions = GetAllowedFileExtensions();
            // Mapper will not notify Blazor that binded values are changed
            // so we need to notify it manually by calling StateHasChanged
            await InvokeAsync(async () =>
            {
                StateHasChanged();
                if (UploadModalRef != null)
                {
                    await UploadModalRef.Show();
                }
            });
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task CheckUploadPolicyAsync()
    {
        await CheckPolicyAsync(UploadPolicyName).ConfigureAwait(false);
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
            UploadModalRef?.Dispose();
        }
        base.Dispose(disposing);
    }
    [Parameter]
    public string AllowedFileExtensions { get; set; }

    protected virtual string GetAllowedFileExtensions()
    {
        return Configuration.FileExtensionsConfiguration.IsNullOrEmpty()
            ? ""
            : Configuration.FileExtensionsConfiguration.Where(x => x.Value)
                .Select(x => x.Key).ToList().JoinAsString(", ");
    }
}