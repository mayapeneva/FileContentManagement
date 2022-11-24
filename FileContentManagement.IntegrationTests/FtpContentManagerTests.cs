using FileContentManagement.DTO;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileContentManagement.IntegrationTests
{
    public class FtpContentManagerTests : IClassFixture<TestFixture>, IDisposable
    {
        private const string FilePath = "../../../TestData/FileOne.docx";
        private readonly Guid fileId;
        private readonly TestFixture factory;

        public FtpContentManagerTests(TestFixture factory)
        {
            fileId = Guid.NewGuid();

            this.factory = factory;
        }

        public void Dispose()
        {
            factory.Dispose();
        }

        [Fact]
        public async Task StoreAsync_StoresValidFile()
        {
            //Arrange
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };

            //Act
            var result = await factory.FtpManager.StoreAsync(fileId, fileInfo, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.False(result.Fail);
            Assert.True(result.SuccessMessages.FirstOrDefault() == $"The file with id {fileId} has been successfully stored.");
        }

        [Fact]
        public async Task StoreAsync_DoesNotStoreEmptyStream()
        {
            //Arrange
            var fileInfo = new StreamInfo
            {
                Length = 0,
                Stream = null
            };

            //Act
            var result = await factory.FtpManager.StoreAsync(fileId, fileInfo, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.True(result.Errors.FirstOrDefault().Message == $"The file with id {fileId} was not stored. Reason: No file content to store.");
        }

        [Fact]
        public async Task StoreAsync_DoesNotStoreClosedStream()
        {
            //Arrange
            var fileInfo = new StreamInfo();
            using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                fileInfo.Length = stream.Length;
                fileInfo.Stream = stream;
            }

            //Act
            var result = await factory.FtpManager.StoreAsync(fileId, fileInfo, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.Contains("Cannot access a closed Stream.", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task StoreAsync_CancelsWithCancellationToken()
        {
            //Arrange
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };

            //Act
            var result = await factory.FtpManager.StoreAsync(fileId, fileInfo, new CancellationToken(true));

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.Contains("A task was canceled.", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task ExistsAsync_FindsExsitingFile()
        {
            //Arrange
            var id = Guid.NewGuid();
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };

            await factory.FtpManager.StoreAsync(id, fileInfo, CancellationToken.None);

            //Act
            var result = await factory.FtpManager.ExistsAsync(id, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.False(result.Fail);
            Assert.True(result.ResultObject);
        }

        [Fact]
        public async Task ExistsAsync_DoesNotFindNonExsitingFile()
        {
            //Act
            var result = await factory.FtpManager.ExistsAsync(new Guid("00000000-0000-0000-0000-000000000000"), CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.False(result.ResultObject);
            Assert.Contains("File unavailable (e.g., file not found, no access).", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task GetAsync_ReturnsExsitingFile()
        {
            //Arrange
            var id = Guid.NewGuid();
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };

            await factory.FtpManager.StoreAsync(id, fileInfo, CancellationToken.None);

            //Act
            var result = await factory.FtpManager.GetAsync(id, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.False(result.Fail);
            Assert.NotNull(result.ResultObject);
            Assert.NotNull(result.ResultObject.Stream);
        }

        [Fact]
        public async Task GetAsync_DoesNotReturnNonExsitingFile()
        {
            //Act
            var result = await factory.FtpManager.GetAsync(new Guid("00000000-0000-0000-0000-000000000000"), CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.Null(result.ResultObject.Stream);
            Assert.Contains("File unavailable (e.g., file not found, no access).", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task GetBytesAsync_ReturnsExsitingFile()
        {
            //Arrange
            var id = Guid.NewGuid();
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };

            await factory.FtpManager.StoreAsync(id, fileInfo, CancellationToken.None);

            //Act
            var result = await factory.FtpManager.GetBytesAsync(id, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.False(result.Fail);
            Assert.NotNull(result.ResultObject);
            Assert.NotEmpty(result.ResultObject);
        }

        [Fact]
        public async Task GetBytesAsync_DoesNotReturnNonExsitingFile()
        {
            //Act
            var result = await factory.FtpManager.GetBytesAsync(new Guid("00000000-0000-0000-0000-000000000000"), CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.Empty(result.ResultObject);
            Assert.Contains("File unavailable (e.g., file not found, no access).", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task DeleteAsync_DeletesExsitingFile()
        {
            //Arrange
            var id = Guid.NewGuid();
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };

            await factory.FtpManager.StoreAsync(id, fileInfo, CancellationToken.None);

            //Act
            var result = await factory.FtpManager.DeleteAsync(id, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.False(result.Fail);
            Assert.True(result.SuccessMessages.FirstOrDefault() == $"The file with id {id} has been successfully deleted.");
        }

        [Fact]
        public async Task DeleteAsync_DoesNotDeleteNonExsitingFile()
        {
            //Arrange
            var id = new Guid("00000000-0000-0000-0000-000000000000");

            //Act
            var result = await factory.FtpManager.DeleteAsync(id, CancellationToken.None); ;

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.Contains("File unavailable (e.g., file not found, no access).", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task GetHashAsync_ReturnsHashForExsitingFile()
        {
            //Arrange
            var id = Guid.NewGuid();
            using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            var fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };

            await factory.FtpManager.StoreAsync(id, fileInfo, CancellationToken.None);

            //Act
            var result = await factory.FtpManager.GetHashAsync(id, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.False(result.Fail);
            Assert.NotNull(result.ResultObject);
            Assert.NotEmpty(result.ResultObject);
        }

        [Fact]
        public async Task GetHashAsync_DoesNotReturnHashForNonExsitingFile()
        {
            //Act
            var result = await factory.FtpManager.GetHashAsync(new Guid("00000000-0000-0000-0000-000000000000"), CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.Empty(result.ResultObject);
            Assert.Contains("File unavailable (e.g., file not found, no access).", result.Errors.FirstOrDefault().Message);
        }
    }
}
