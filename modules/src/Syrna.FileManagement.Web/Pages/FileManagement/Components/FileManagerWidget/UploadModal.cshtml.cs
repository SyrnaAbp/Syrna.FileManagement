using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syrna.FileManagement.Containers;
using Syrna.FileManagement.Files;
using Syrna.FileManagement.Files.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget;

public class UploadModalModel : FileManagementPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string FileContainerName { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? OwnerUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? ParentId { get; set; }

    [BindProperty]
    public IFormFile[] UploadedFiles { get; set; }

    public PublicFileContainerConfiguration Configuration { get; set; }

    private readonly IFileAppService _service;

    public UploadModalModel(IFileAppService service)
    {
        _service = service;
    }

    public virtual async Task OnGetAsync()
    {
        Configuration = await _service.GetConfigurationAsync(FileContainerName, OwnerUserId);
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var dto = new CreateManyFileWithStreamInput
        {
            FileContainerName = FileContainerName,
            OwnerUserId = OwnerUserId,
            ParentId = ParentId,
        };
        foreach (var uploadedFile in UploadedFiles)
        {
            dto.FileContents.Add(new RemoteStreamContent(
                stream: uploadedFile.OpenReadStream(),
                fileName: uploadedFile.FileName,
                contentType: uploadedFile.ContentType));
        }

        await _service.CreateManyWithStreamAsync(dto);

        return NoContent();
    }

    public virtual string GetAllowedFileExtensionsJsCode()
    {
        return Configuration.FileExtensionsConfiguration.IsNullOrEmpty()
            ? "[]"
            : "['" + Configuration.FileExtensionsConfiguration.Where(x => x.Value)
                .Select(x => x.Key.ReplaceFirst(".", "")).ToList().JoinAsString("' ,'") + "']";
    }
}