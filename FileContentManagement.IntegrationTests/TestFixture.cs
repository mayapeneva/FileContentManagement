using FileContentManagement.Configuration;
using System;
using System.IO;

namespace FileContentManagement.IntegrationTests
{
    public class TestFixture : IDisposable
    {
        private readonly FtpServer server;

        public TestFixture()
        {
            server = new FtpServer(string.Empty);

            var fileManagementConfiguration = new FileManagementConfiguration("172.22.64.1", 21, "anonymous", "mozilla@example.com");
            FtpManager = new FtpContentManager<Guid>(fileManagementConfiguration);
        }

        public IContentManager<Guid> FtpManager { get; set; }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}
