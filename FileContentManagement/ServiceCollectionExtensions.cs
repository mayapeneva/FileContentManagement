using Microsoft.Extensions.DependencyInjection;

namespace FileContentManagement
{
    public class ServiceCollectionExtensions
    {
        public static IServiceCollection Register(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .RegisterSettings<FtpSettings>(configuration)
                .AddSingleton<IFileStorage, FtpClient>();

            return services;
        }
    }
}
