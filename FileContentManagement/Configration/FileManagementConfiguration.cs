namespace FileContentManagement.Configuration
{
    public class FileManagementConfiguration
    {
        public FileManagementConfiguration(string host, int port, string username, string password)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }
    }
}
