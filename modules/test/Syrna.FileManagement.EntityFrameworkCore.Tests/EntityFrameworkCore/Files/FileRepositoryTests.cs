using System;
using System.Threading.Tasks;
using Syrna.FileManagement.Files;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Syrna.FileManagement.EntityFrameworkCore.Files
{
    public class FileRepositoryTests : FileManagementEntityFrameworkCoreTestBase
    {
        private readonly IFileRepository _fileRepository;

        public FileRepositoryTests()
        {
            _fileRepository = GetRequiredService<IFileRepository>();
        }

        [Fact]
        public async Task Test1()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange

                // Act

                //Assert
            });
        }
    }
}
