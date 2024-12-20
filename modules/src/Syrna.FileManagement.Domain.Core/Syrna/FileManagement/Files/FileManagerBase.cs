﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Syrna.FileManagement.Containers;
using Syrna.FileManagement.Options.Containers;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace Syrna.FileManagement.Files;

public abstract class FileManagerBase : DomainService, IFileManager
{
    protected IFileRepository FileRepository => LazyServiceProvider.LazyGetRequiredService<IFileRepository>();

    protected IDistributedEventBus DistributedEventBus =>
        LazyServiceProvider.LazyGetRequiredService<IDistributedEventBus>();

    protected IUnitOfWorkManager UnitOfWorkManager =>
        LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();

    protected IFileContainerConfigurationProvider FileContainerConfigurationProvider =>
        LazyServiceProvider.LazyGetRequiredService<IFileContainerConfigurationProvider>();

    public abstract Task<File> CreateAsync(CreateFileModel model, CancellationToken cancellationToken = default);

    public abstract Task<File> CreateAsync(CreateFileWithStreamModel model,
        CancellationToken cancellationToken = default);

    public abstract Task<List<File>> CreateManyAsync(List<CreateFileModel> models,
        CancellationToken cancellationToken = default);

    public abstract Task<List<File>> CreateManyAsync(List<CreateFileWithStreamModel> models,
        CancellationToken cancellationToken = default);

    public virtual async Task<File> GetByPathAsync(string path, string fileContainerName, Guid? ownerUserId)
    {
        var foundFile = await FindByPathAsync(path, fileContainerName, ownerUserId);

        if (foundFile is null)
        {
            throw new EntityNotFoundException(typeof(File), path);
        }

        return foundFile;
    }

    public virtual async Task<File> FindByPathAsync(string path, string fileContainerName, Guid? ownerUserId)
    {
        Check.NotNullOrWhiteSpace(path, nameof(path));
        Check.NotNullOrWhiteSpace(fileContainerName, nameof(fileContainerName));

        var splitFileName = path.Split(FileManagementConsts.DirectorySeparator);

        foreach (var fileName in splitFileName)
        {
            Check.Length(fileName, "fileName", FileManagementConsts.File.FileNameMaxLength, 1);
        }

        File foundFile = null;
        Guid? parentId = null;

        foreach (var fileName in splitFileName)
        {
            foundFile = await FileRepository.FindAsync(fileName, parentId, fileContainerName, ownerUserId, false);

            if (foundFile is null)
            {
                return null;
            }

            parentId = foundFile.Id;
        }

        return foundFile;
    }

    public virtual async Task<FileLocationModel> GetFileLocationAsync(File file,
        CancellationToken cancellationToken = default)
    {
        var isDirectory = file.FileType == FileType.Directory;

        if (!file.ParentId.HasValue)
        {
            return new FileLocationModel(isDirectory, string.Empty, file.FileName);
        }

        var parent = await FileRepository.GetAsync(file.ParentId.Value, true, cancellationToken);

        var parentLocation = await GetFileLocationAsync(parent, cancellationToken);

        return new FileLocationModel(isDirectory, parentLocation.FilePath, file.FileName);
    }

    protected abstract IFileDownloadProvider GetFileDownloadProvider(File file);

    public virtual async Task<FileDownloadInfoModel> GetDownloadInfoAsync(File file)
    {
        if (file.FileType != FileType.RegularFile)
        {
            throw new UnexpectedFileTypeException(file.Id, file.FileType, FileType.RegularFile);
        }

        var provider = GetFileDownloadProvider(file);

        return await provider.CreateDownloadInfoAsync(file);
    }

    [UnitOfWork(true)]
    public virtual async Task<File> UpdateInfoAsync(File file, UpdateFileInfoModel model,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(model.NewFileName, nameof(File.FileName));

        var parent = await TryGetFileByNullableIdAsync(file.ParentId);

        var configuration = FileContainerConfigurationProvider.Get(file.FileContainerName);

        CheckFileName(model.NewFileName, configuration);

        if (file.FileType == FileType.RegularFile)
        {
            CheckFileExtension(new[] { model.NewFileName }, configuration);
        }

        if (model.NewFileName != file.FileName)
        {
            await CheckFileNotExistAsync(model.NewFileName, file.ParentId, file.FileContainerName,
                file.OwnerUserId, true);
        }

        file.UpdateInfo(model.NewFileName, model.NewMimeType, file.SubFilesQuantity, file.HasSubdirectories,
            file.ByteSize, file.Hash, file.BlobName, parent);

        await FileRepository.UpdateAsync(file, true, cancellationToken);

        return file;
    }

    [UnitOfWork(true)]
    public virtual async Task<File> MoveAsync(File file, MoveFileModel model,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(model.NewFileName, nameof(File.FileName));

        var oldParent = model.NewParent?.Id == file.ParentId
            ? model.NewParent
            : await TryGetFileByNullableIdAsync(file.ParentId);
        var newParent = model.NewParent;

        var configuration = FileContainerConfigurationProvider.Get(file.FileContainerName);

        CheckFileName(model.NewFileName, configuration);

        if (file.FileType == FileType.RegularFile)
        {
            CheckFileExtension(new[] { model.NewFileName }, configuration);
        }

        if (model.NewFileName != file.FileName || newParent?.Id != oldParent?.Id)
        {
            await CheckFileNotExistAsync(model.NewFileName, newParent?.Id, file.FileContainerName,
                file.OwnerUserId, true);
        }

        if (oldParent != newParent)
        {
            await CheckNotMovingDirectoryToSubDirectoryAsync(file, newParent);
        }

        file.UpdateInfo(model.NewFileName, file.MimeType, file.SubFilesQuantity, file.HasSubdirectories, file.ByteSize,
            file.Hash, file.BlobName, newParent);

        if (oldParent is not null)
        {
            await HandleStatisticDataUpdateAsync(oldParent.Id);
        }

        if (newParent is not null)
        {
            await HandleStatisticDataUpdateAsync(newParent.Id);
        }

        await FileRepository.UpdateAsync(file, true, cancellationToken);

        return file;
    }

    [UnitOfWork(true)]
    public virtual async Task DeleteAsync([NotNull] File file, CancellationToken cancellationToken = default)
    {
        var parent = file.ParentId.HasValue
            ? await FileRepository.GetAsync(file.ParentId.Value, true, cancellationToken)
            : null;

        if (parent is not null)
        {
            await HandleStatisticDataUpdateAsync(parent.Id);
        }

        // It is used to accommodate both soft deletion and unique indexing.
        file.SoftDeletionToken = Guid.NewGuid().ToString();
        await FileRepository.UpdateAsync(file, true, cancellationToken);

        await FileRepository.DeleteAsync(file, true, cancellationToken);

        if (file.FileType == FileType.Directory)
        {
            await DeleteSubFilesAsync(file, file.FileContainerName, file.OwnerUserId, cancellationToken);
        }
    }

    protected virtual async Task DeleteSubFilesAsync([CanBeNull] File file, [NotNull] string fileContainerName,
        Guid? ownerUserId, CancellationToken cancellationToken = default)
    {
        var subFiles = await FileRepository.GetListAsync(file?.Id, fileContainerName, ownerUserId,
            null, cancellationToken);

        foreach (var subFile in subFiles)
        {
            if (subFile.FileType == FileType.Directory)
            {
                await DeleteSubFilesAsync(subFile, fileContainerName, ownerUserId, cancellationToken);
            }

            // It is used to accommodate both soft deletion and unique indexing.
            subFile.SoftDeletionToken = Guid.NewGuid().ToString();
            await FileRepository.UpdateAsync(subFile, true, cancellationToken);

            await FileRepository.DeleteAsync(subFile, true, cancellationToken);
        }
    }

    protected virtual async Task<File> TryGetFileByNullableIdAsync(Guid? fileId)
    {
        return fileId.HasValue ? await FileRepository.GetAsync(fileId.Value) : null;
    }

    protected virtual void CheckDirectoryHasNoFileContent(FileType fileType, long fileContentLength)
    {
        if (fileType == FileType.Directory && fileContentLength > 0)
        {
            throw new DirectoryFileContentIsNotEmptyException();
        }
    }

    [UnitOfWork(true)]
    protected virtual Task HandleStatisticDataUpdateAsync(Guid directoryId)
    {
        var useBackgroundJob = DistributedEventBus is LocalDistributedEventBus;

        UnitOfWorkManager.Current.AddOrReplaceDistributedEvent(
            new UnitOfWorkEventRecord(
                typeof(SubFilesChangedEto),
                new SubFilesChangedEto(CurrentTenant.Id, directoryId, useBackgroundJob),
                default));

        return Task.CompletedTask;
    }

    protected virtual async Task CheckNotMovingDirectoryToSubDirectoryAsync([NotNull] File file,
        [CanBeNull] File targetParent)
    {
        if (file.FileType != FileType.Directory)
        {
            return;
        }

        var parent = targetParent;

        while (parent != null)
        {
            if (parent.Id == file.Id)
            {
                throw new FileIsMovingToSubDirectoryException();
            }

            parent = parent.ParentId.HasValue ? await FileRepository.GetAsync(parent.ParentId.Value) : null;
        }
    }

    protected virtual void CheckFileName(string fileName, IFileContainerConfiguration configuration)
    {
        Check.NotNullOrWhiteSpace(fileName, nameof(File.FileName));

        if (fileName.Contains(FileManagementConsts.DirectorySeparator))
        {
            throw new FileNameContainsSeparatorException(fileName, FileManagementConsts.DirectorySeparator);
        }
    }

    [UnitOfWork]
    protected virtual async Task<bool> IsFileExistAsync(string fileName, Guid? parentId, string fileContainerName,
        Guid? ownerUserId, bool forceCaseSensitive)
    {
        return await FileRepository.ExistAsync(fileName, parentId, fileContainerName, ownerUserId, forceCaseSensitive);
    }

    protected virtual async Task CheckFileNotExistAsync(string fileName, Guid? parentId, string fileContainerName,
        Guid? ownerUserId, bool forceCaseSensitive)
    {
        if (await IsFileExistAsync(fileName, parentId, fileContainerName, ownerUserId, forceCaseSensitive))
        {
            throw new FileAlreadyExistsException(fileName, parentId);
        }
    }

    protected virtual void CheckFileQuantity(int count, IFileContainerConfiguration configuration)
    {
        if (count > configuration.MaxFileQuantityForEachUpload)
        {
            throw new UploadQuantityExceededLimitException(count, configuration.MaxFileQuantityForEachUpload);
        }
    }

    protected virtual void CheckFileSize(List<(string, long)> fileNameByteSizes,
        IFileContainerConfiguration configuration)
    {
        foreach (var tuple in fileNameByteSizes.Where(tuple => tuple.Item2 > configuration.MaxByteSizeForEachFile))
        {
            throw new FileSizeExceededLimitException(tuple.Item1, tuple.Item2, configuration.MaxByteSizeForEachFile);
        }

        var totalByteSize = fileNameByteSizes.Select(x => x.Item2).Sum();

        if (totalByteSize > configuration.MaxByteSizeForEachUpload)
        {
            throw new UploadSizeExceededLimitException(totalByteSize, configuration.MaxByteSizeForEachUpload);
        }
    }

    protected virtual void CheckFileExtension(IEnumerable<string> fileNames, IFileContainerConfiguration configuration)
    {
        foreach (var fileName in fileNames.Where(fileName => !IsFileExtensionAllowed(fileName, configuration)))
        {
            throw new FileExtensionIsNotAllowedException(fileName);
        }
    }

    protected virtual bool IsFileExtensionAllowed(string fileName, IFileContainerConfiguration configuration)
    {
        var lowerFileName = fileName.ToLowerInvariant();

        foreach (var pair in configuration.FileExtensionsConfiguration.Where(x =>
                     lowerFileName.EndsWith(x.Key.ToLowerInvariant())))
        {
            return pair.Value;
        }

        return !configuration.AllowOnlyConfiguredFileExtensions;
    }
}