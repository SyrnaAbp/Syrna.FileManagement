using System;
using System.ComponentModel.DataAnnotations;

namespace Syrna.FileManagement.Blazor.ViewModels;

public class MoveFileViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "FileParentId")]
    public string NewParentId { get; set; }

    [Required]
    [Display(Name = "FileFileName")]
    public string NewFileName { get; set; }
}