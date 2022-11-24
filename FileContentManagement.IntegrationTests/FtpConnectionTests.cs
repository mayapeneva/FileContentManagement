using FileContentManagement.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileContentManagement.IntegrationTests
{
    public class FtpConnectionTests
    {
        private readonly Guid fileId;
        public FtpConnectionTests()
        {
            fileId = Guid.NewGuid();
        }

        [Fact]
        public async Task ExistsAsync_CannotLogInTheFtpWithWrongUser()
        {
            //Arrange
            var fileManagementConfiguration = new FileManagementConfiguration("192.168.0.103", 21, "user", "pass123");
            var ftpManager = new FtpContentManager<Guid>(fileManagementConfiguration);

            //Act
            var result = await ftpManager.ExistsAsync(fileId, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.False(result.ResultObject);
            Assert.Contains("The remote server returned an error: (530) Not logged in.", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task ExistsAsync_CannotLogInTheFtpWithWrongPassword()
        {
            //Arrange
            var fileManagementConfiguration = new FileManagementConfiguration("192.168.0.103", 21, "FTP-user", "pass");
            var ftpManager = new FtpContentManager<Guid>(fileManagementConfiguration);

            //Act
            var result = await ftpManager.ExistsAsync(fileId, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.False(result.ResultObject);
            Assert.Contains("The remote server returned an error: (530) Not logged in.", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task ExistsAsync_CannotLogInTheFtpWithWrongPort()
        {
            //Arrange
            var fileManagementConfiguration = new FileManagementConfiguration("192.168.0.103", 22, "FTP-user", "pass123");
            var ftpManager = new FtpContentManager<Guid>(fileManagementConfiguration);

            //Act
            var result = await ftpManager.ExistsAsync(fileId, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.False(result.ResultObject);
            Assert.Contains("Unable to connect to the remote server", result.Errors.FirstOrDefault().Message);
        }

        [Fact]
        public async Task ExistsAsync_CannotLogInTheFtpWithWrongHost()
        {
            //Arrange
            var fileManagementConfiguration = new FileManagementConfiguration("192.168.0.102", 21, "FTP-user", "pass123");
            var ftpManager = new FtpContentManager<Guid>(fileManagementConfiguration);

            //Act
            var result = await ftpManager.ExistsAsync(fileId, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.True(result.Fail);
            Assert.False(result.ResultObject);
            Assert.Contains("Unable to connect to the remote server", result.Errors.FirstOrDefault().Message);
        }
    }
}
