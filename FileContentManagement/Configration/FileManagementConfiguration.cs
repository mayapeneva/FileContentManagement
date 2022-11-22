namespace FileContentManagement.Configuration
{
    internal class FileManagementConfiguration
    {
        public FileManagementConfiguration(string host, int port, string username, string password)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
