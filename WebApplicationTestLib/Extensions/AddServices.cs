using WebApplicationTestLib.Options;
using WebApplicationTestLib.Repositories;

namespace WebApplicationTestLib.Extensions
{
    public static class AddServices
    {
        public static IServiceCollection AddCategoryRepository(this IServiceCollection services, Action<DatabaseOptions> config)
        {
            services.Configure(config);
            services.AddTransient<CategoryRepository>();
            return services;
        }
    }
}
