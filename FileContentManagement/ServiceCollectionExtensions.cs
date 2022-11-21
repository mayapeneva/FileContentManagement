using Microsoft.Extensions.DependencyInjection;
using System;

namespace FileContentManagement
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterContentManager<TKey>(this IServiceCollection services)
        where TKey : struct, IEquatable<TKey>
        {
            services
                .AddSingleton<IContentManager<TKey>, ContentManager<TKey>>();

            return services;
        }
    }
}
