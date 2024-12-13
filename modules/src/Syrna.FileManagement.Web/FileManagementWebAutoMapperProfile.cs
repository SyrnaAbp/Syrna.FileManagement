using AutoMapper;
using Syrna.FileManagement.Files.Dtos;
using Syrna.FileManagement.Web.Pages.FileManagement.Components.FileManagerWidget.ViewModels;

namespace Syrna.FileManagement.Web
{
    public class FileManagementWebAutoMapperProfile : Profile
    {
        public FileManagementWebAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<FileInfoDto, RenameFileViewModel>();
        }
    }
}