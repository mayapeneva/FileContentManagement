using FileContentManagement.DTO;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileContentManagement.IntegrationTests
{
    public class FtpContentManagerTests : IDisposable
    {
        private readonly DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "FileContentManagement.IntegrationTests"));
        private readonly FtpServer server;
        private readonly IContentManager<Guid> ftpManager;

        private readonly Guid fileId;
        private readonly StreamInfo fileInfo;

        public FtpContentManagerTests(IContentManager<Guid> ftpManager)
        {
            server = new FtpServer(dir.FullName);
            this.ftpManager = ftpManager;

            fileId = Guid.NewGuid();
            using var stream = new FileStream("TestData/FileOne.docx", FileMode.Open, FileAccess.Read);
            fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };
        }

        public void Dispose()
        {
            server.Dispose();
            dir.Delete(true);
        }

        [Fact]
        public async Task StoreAsync_StoresValidFile()
        {
            //var id = Guid.NewGuid();
            //using var stream = new FileStream("TestData/FileOne.docx", FileMode.Open, FileAccess.Read);
            //var info = new StreamInfo
            //{
            //    Length = stream.Length,
            //    Stream = stream
            //};

            var result = await ftpManager.StoreAsync(fileId, fileInfo, CancellationToken.None);

            Assert.True(result.Success);
            Assert.False(result.Fail);
            Assert.True(result.SuccessMessages.FirstOrDefault() == $"The file with id {fileId} has been successfully stored.");
        }
    }
}
