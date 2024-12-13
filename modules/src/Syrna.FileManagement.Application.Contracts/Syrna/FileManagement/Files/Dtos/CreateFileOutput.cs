using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

namespace Syrna.FileManagement.Files.Dtos
{
    [Serializable]
    public class CreateFileOutput : ExtensibleObject
    {
        public FileInfoDto FileInfo { get; set; }
        
        public FileDownloadInfoModel DownloadInfo { get; set; }
    }
}