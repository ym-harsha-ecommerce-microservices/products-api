using eCommerce.DAL.DbContexts;
using eCommerce.DAL.Repositories.Contracts;
using eCommerce.DAL.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.DAL;

/// <summary>
/// Provides an extension method for registering the Data Access Layer's
/// database context and repositories into the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Data Access Layer dependencies: configures <see cref="ApplicationDbContext"/>
    /// to connect to MySQL using a connection string built from configuration with
    /// server/user/password values substituted from environment variables, and
    /// registers repository implementations.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <param name="configuration">The application configuration containing the connection string template.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, for chaining.</returns>
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var server = Environment.GetEnvironmentVariable("DB_SERVER");
            var user = Environment.GetEnvironmentVariable("DB_USER");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var databaseName = Environment.GetEnvironmentVariable("DB_NAME");

            var connectionString = configuration.GetConnectionString("MySqlConnection")!
                .Replace("$DB_SERVER", server)
                .Replace("$DB_USER", user)
                .Replace("$DB_PASSWORD", password)
                .Replace("$DB_NAME", databaseName);
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        services.AddScoped<IProductsRepository, ProductsRepository>();

        return services;
    }
}