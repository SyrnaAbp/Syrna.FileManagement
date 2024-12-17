using AutoMapper;
using Syrna.FileManagement.Blazor.ViewModels;
using Syrna.FileManagement.Files.Dtos;

namespace Syrna.FileManagement.Blazor
{
    public class FileManagementBlazorAutoMapperProfile : Profile
    {
        public FileManagementBlazorAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<FileInfoDto, RenameFileViewModel>();
            CreateMap<FileInfoDto, FileInfoViewModel>();
        }
    }
}