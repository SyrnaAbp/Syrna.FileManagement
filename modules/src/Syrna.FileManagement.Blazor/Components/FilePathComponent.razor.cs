using Microsoft.AspNetCore.Components;
using Syrna.FileManagement.Blazor.ViewModels;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Syrna.FileManagement.Blazor.Components;

public partial class FilePathComponent : ComponentBase
{
    [Parameter]
    public object Data { get; set; }

    protected TextFileContentModal TextFileContentModalRef;
    protected ImageFileContentModal ImageFileContentModalRef;

    private FileInfoViewModel FileInfo => Data.As<FileInfoViewModel>();

    protected virtual async Task FolderClickedAsync()
    {
        await FileInfo.ParentIdChanged(FileInfo.Id);
    }

    protected virtual async Task FileClickedAsync()
    {
        if (SharedFunctions.IsTextFile(FileInfo.MimeType))
            await TextFileContentModalRef.OpenAsync(FileInfo.Id);
        else if (SharedFunctions.IsImageFile(FileInfo.MimeType))
            await ImageFileContentModalRef.OpenAsync(FileInfo.Id);
        else
        {
            throw new UserFriendlyException("Unsupported mime type");
        }
    }
}
