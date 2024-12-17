using System;
using System.ComponentModel.DataAnnotations;

namespace Syrna.FileManagement.Blazor.ViewModels;

public class CreateDirectoryViewModel
{
    public string FileContainerName { get; internal set; }
    
    public Guid? OwnerUserId { get; internal set; }
    
    public string DirectoryName { get; internal set; }
   
    public Guid? ParentId { get; internal set; }
}
