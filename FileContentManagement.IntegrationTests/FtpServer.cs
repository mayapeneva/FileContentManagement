using System;
using System.Diagnostics;

namespace FileContentManagement.IntegrationTests
{
    public class FtpServer : IDisposable
    {
        private readonly Process ftpProcess;

        public FtpServer(string rootDirectory, int port = 21, bool hideFtpWindow = true)
        {
            var psInfo = new ProcessStartInfo
            {
                FileName = "ftpdmin.exe",
                Arguments = String.Format("-p {0} -ha 127.0.0.1 \"{1}\"", port, rootDirectory),
                WindowStyle = hideFtpWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal
            };

            ftpProcess = Process.Start(psInfo);
        }

        public void Dispose()
        {
            if (!ftpProcess.HasExited)
            {
                ftpProcess.Kill();
                ftpProcess.WaitForExit();
            }
        }
    }
}
