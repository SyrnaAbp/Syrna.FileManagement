using System;
using JetBrains.Annotations;

namespace Syrna.FileManagement.Files;

public interface IFileOperationInfoModel
{
    [NotNull]
    string FileContainerName { get; }

    Guid? OwnerUserId { get; }
}