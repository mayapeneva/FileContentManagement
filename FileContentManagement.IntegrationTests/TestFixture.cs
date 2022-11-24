using FileContentManagement.Configuration;
using System;
using System.IO;

namespace FileContentManagement.IntegrationTests
{
    public class TestFixture : IDisposable
    {
        //private readonly FtpServer server;

        public TestFixture()
        {
            //server = new FtpServer(string.Empty);

            var fileManagementConfiguration = new FileManagementConfiguration("192.168.0.103", 21, "FTP-user", "pass123");
            FtpManager = new FtpContentManager<Guid>(fileManagementConfiguration);
        }

        public IContentManager<Guid> FtpManager { get; set; }

        public void Dispose()
        {
            //server.Dispose();
        }
    }
}
