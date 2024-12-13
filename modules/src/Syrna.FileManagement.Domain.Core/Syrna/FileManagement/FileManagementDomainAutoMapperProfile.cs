using AutoMapper;
using Syrna.FileManagement.Files;

namespace Syrna.FileManagement
{
    public class FileManagementDomainAutoMapperProfile : Profile
    {
        public FileManagementDomainAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<File, FileEto>();
        }
    }
}
