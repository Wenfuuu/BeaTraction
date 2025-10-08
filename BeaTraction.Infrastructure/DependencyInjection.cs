using BeaTraction.Domain.Interfaces;
using BeaTraction.Infrastructure.Persistence;
using BeaTraction.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaTraction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = BuildConnectionString(configuration);

        // Add DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IAttractionRepository, AttractionRepository>();
        services.AddScoped<IRegistrationRepository, RegistrationRepository>();

        return services;
    }
    
    private static string BuildConnectionString(IConfiguration configuration)
    {
        var existingConnectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (!string.IsNullOrEmpty(existingConnectionString))
        {
            return existingConnectionString;
        }

        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "db_name";
        var username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "postgres";

        return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
    }
}
