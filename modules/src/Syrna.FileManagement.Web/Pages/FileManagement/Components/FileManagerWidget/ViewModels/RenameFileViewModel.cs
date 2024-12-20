using System.ComponentModel.DataAnnotations;

namespace Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;

public class RenameFileViewModel
{
    [Required]
    [Display(Name = "FileFileName")]
    public string FileName { get; set; }
}