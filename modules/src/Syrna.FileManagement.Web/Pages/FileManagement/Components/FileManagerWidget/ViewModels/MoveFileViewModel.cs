using System;
using System.ComponentModel.DataAnnotations;

namespace Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;

public class MoveFileViewModel
{
    [Display(Name = "FileParentId")]
    public Guid? NewParentId { get; set; }
        
    [Required]
    [Display(Name = "FileFileName")]
    public string NewFileName { get; set; }
}