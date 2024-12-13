using System;
using System.Threading.Tasks;
using Syrna.FileManagement.Files;
using Syrna.FileManagement.Files.Dtos;
using Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class RenameModalModel : FileManagementPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public RenameFileViewModel ViewModel { get; set; }

        
    private readonly IFileAppService _service;

    public RenameModalModel(IFileAppService service)
    {
        _service = service;
    }

    public virtual async Task OnGetAsync()
    {
        var dto = await _service.GetAsync(Id);
        ViewModel = ObjectMapper.Map<FileInfoDto, RenameFileViewModel>(dto);
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var dto = new UpdateFileInfoInput
        {
            FileName = ViewModel.FileName,
        };
            
        await _service.UpdateInfoAsync(Id, dto);

        return NoContent();
    }
}