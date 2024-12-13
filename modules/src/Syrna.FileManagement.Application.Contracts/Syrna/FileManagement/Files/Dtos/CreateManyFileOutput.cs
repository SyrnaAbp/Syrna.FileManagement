using System;
using Volo.Abp.Application.Dtos;

namespace Syrna.FileManagement.Files.Dtos
{
    [Serializable]
    public class CreateManyFileOutput : ListResultDto<CreateFileOutput>
    {
    }
}