using Syrna.FileManagement.Files;
using Syrna.FileManagement.Files.Dtos;
using AutoMapper;
using Volo.Abp.AutoMapper;

namespace Syrna.FileManagement
{
    public class FileManagementApplicationAutoMapperProfile : Profile
    {
        public FileManagementApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            
            CreateMap<File, FileInfoDto>()
                .Ignore(x => x.Owner)
                .Ignore(x => x.Creator)
                .Ignore(x => x.LastModifier)
                .MapExtraProperties();
        }
    }
}
