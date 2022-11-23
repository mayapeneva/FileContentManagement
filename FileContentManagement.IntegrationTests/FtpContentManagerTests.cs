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
        private readonly Guid fileId;
        private readonly StreamInfo fileInfo;
        private readonly TestFixture factory;

        public FtpContentManagerTests(TestFixture factory)
        {
            fileId = Guid.NewGuid();
            using var stream = new FileStream("../../../TestData/FileOne.docx", FileMode.Open, FileAccess.Read);
            fileInfo = new StreamInfo
            {
                Length = stream.Length,
                Stream = stream
            };

            this.factory = factory;
        }
        public void Dispose()
        {
            factory.Dispose();
        }

        [Fact]
        public async Task StoreAsync_StoresValidFile()
        {
            var result = await factory.FtpManager.StoreAsync(fileId, fileInfo, CancellationToken.None);

            Assert.True(result.Success);
            Assert.False(result.Fail);
            Assert.True(result.SuccessMessages.FirstOrDefault() == $"The file with id {fileId} has been successfully stored.");
        }
    }
}
