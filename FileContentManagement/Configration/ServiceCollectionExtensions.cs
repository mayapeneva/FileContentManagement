using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FileContentManagement.Configuration
{
    public static class ServiceCollectionExtensions
    {
        private const string SectionName = "fileManagement";
        private const string Host = "host";
        private const string Port = "port";
        private const string Username = "username";
        private const string Password = "password";

        public static IServiceCollection RegisterContentManager<TKey>(this IServiceCollection services, IConfiguration configuration)
        where TKey : struct, IEquatable<TKey>
        {
            var host = configuration[$"{SectionName}:{Host}"];
            var isPortParsed = int.TryParse(configuration[$"{SectionName}:{Port}"], out int port);
            var username = configuration[$"{SectionName}:{Username}"];
            var password = configuration[$"{SectionName}:{Password}"];
            if (string.IsNullOrWhiteSpace(host)
                || !isPortParsed
                || string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(SectionName);
            }

            var fileManagementConfiguration = new FileManagementConfiguration(host, port, username, password);
            services
                .AddSingleton(fileManagementConfiguration)
                .AddSingleton<IContentManager<TKey>, ContentManager<TKey>>();

            return services;
        }
    }
}
