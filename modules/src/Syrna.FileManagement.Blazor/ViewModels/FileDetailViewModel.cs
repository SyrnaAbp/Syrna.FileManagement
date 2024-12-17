using System;
using System.ComponentModel.DataAnnotations;
using Syrna.FileManagement.Files;

namespace Syrna.FileManagement.Blazor.ViewModels;

public class FileDetailViewModel
{
    [Display(Name = "FileId")]
    public string Id { get; set; }

    //[HiddenInput]
    [Display(Name = "FileFileName")]
    public string FileName { get; set; }

    //[DisabledInput]
    [Display(Name = "FileFileType")]
    public FileType FileType { get; set; }

    //[DisabledInput]
    [Display(Name = "FileMimeType")]
    public string MimeType { get; set; }

    //[DisabledInput]
    [Display(Name = "FileByteSize")]
    public string ByteSize { get; set; }

    //[DisabledInput]
    [Display(Name = "FileHash")]
    public string Hash { get; set; }

    //[DisabledInput]
    [Display(Name = "Location")]
    public string Location { get; set; }

    //[DisabledInput]
    [Display(Name = "Creator")]
    public string Creator { get; set; }

    //[DisabledInput]
    [Display(Name = "CreationTime")]
    public string Created { get; set; }

    //[DisabledInput]
    [Display(Name = "LastModifier")]
    public string LastModifier { get; set; }

    //[DisabledInput]
    [Display(Name = "LastModificationTime")]
    public string Modified { get; set; }
}