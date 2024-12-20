using System;
using Syrna.FileManagement.Users;
using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;

namespace Syrna.FileManagement.Files.Dtos
{
    [Serializable]
    public class FileInfoDto : ExtensibleFullAuditedEntityDto<Guid>
    {
        public Guid? ParentId { get; set; }

        public string FileContainerName { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        public FileType FileType { get; set; }

        public int SubFilesQuantity { get; set; }

        public bool HasSubdirectories { get; set; }

        public long ByteSize { get; set; }

        public string Hash { get; set; }

        public Guid? OwnerUserId { get; set; }

        [CanBeNull]
        public BriefFileUserInfoModel Owner { get; set; }

        [CanBeNull]
        public BriefFileUserInfoModel Creator { get; set; }

        [CanBeNull]
        public BriefFileUserInfoModel LastModifier { get; set; }
    }
}