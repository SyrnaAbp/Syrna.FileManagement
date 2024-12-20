using System;
using System.ComponentModel.DataAnnotations;

namespace Syrna.FileManagement.Blazor.ViewModels;

public class RenameFileViewModel
{
    public Guid Id { get; set; }
    
    [Required]
    [Display(Name = "FileFileName")]
    public string FileName { get; set; }
}
