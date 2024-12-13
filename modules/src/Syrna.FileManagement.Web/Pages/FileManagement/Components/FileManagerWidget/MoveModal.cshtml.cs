using System;
using System.Threading.Tasks;
using Syrna.FileManagement.Files;
using Syrna.FileManagement.Files.Dtos;
using Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class MoveModalModel : FileManagementPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public MoveFileViewModel ViewModel { get; set; }

    private readonly IFileAppService _service;

    public MoveModalModel(IFileAppService service)
    {
        _service = service;
    }

    public virtual async Task OnGetAsync()
    {
        var dto = await _service.GetAsync(Id);
            
        ViewModel = new MoveFileViewModel
        {
            NewFileName = dto.FileName
        };
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var dto = new MoveFileInput
        {
            NewParentId = ViewModel.NewParentId,
            NewFileName = ViewModel.NewFileName
        };
            
        await _service.MoveAsync(Id, dto);
            
        return NoContent();
    }
}