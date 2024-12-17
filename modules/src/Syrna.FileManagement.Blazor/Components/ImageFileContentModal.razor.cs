using System;
using System.Text;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Syrna.FileManagement.Files;
using Syrna.FileManagement.Localization;

namespace Syrna.FileManagement.Blazor.Components;

public partial class ImageFileContentModal
{
    public string Content { get; set; }

    protected Modal _modal;

    [Inject]
    protected IFileAppService FileAppService { get; set; }

    [Inject]
    protected IFileContentAppService FileContentAppService { get; set; }

    public ImageFileContentModal()
    {
        LocalizationResource = typeof(FileManagementResource);
    }

    public virtual async Task OpenAsync(Guid id)
    {
        try
        {
            var fileInfo = await FileAppService.GetDownloadInfoAsync(id);
            if (fileInfo != null)
            {
                var fileData = await FileAppService.DownloadAsync(id,fileInfo.Token);
                if (fileData == null)
                {
                    Content = "ERROR";
                }
                else
                {
                    Content = $"data:{fileData.MimeType};base64,{System.Convert.ToBase64String(fileData.Content)}";
                }
                await InvokeAsync(_modal.Show);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected Task CloseModal()
    {
        return InvokeAsync(_modal.Hide);
    }

    protected virtual Task ClosingModal(ModalClosingEventArgs eventArgs)
    {
        eventArgs.Cancel = eventArgs.CloseReason == CloseReason.FocusLostClosing;
        return Task.CompletedTask;
    }
}
