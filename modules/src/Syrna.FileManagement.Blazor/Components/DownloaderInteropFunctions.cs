using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Syrna.FileManagement.Blazor.Components;

public static class DownloaderInteropFunctions
{
    public static async Task TriggerClickEvent(this ElementReference elementRef,
        IJSRuntime js)
    {
        await js.InvokeVoidAsync("downloaderInteropFunctions.clickElement", elementRef);
    }
}
