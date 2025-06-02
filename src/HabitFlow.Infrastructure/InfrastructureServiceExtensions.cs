using System;
using HabitFlow.Application.Common.Interfaces;
using HabitFlow.Domain.Repositories;
using HabitFlow.Infrastructure.Persistence;
using HabitFlow.Infrastructure.Repositories;
using HabitFlow.Infrastructure.Services;
using HabitFlow.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HabitFlow.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection");
        var mysqlServerVersion = new MySqlServerVersion(ServerVersion.AutoDetect(connectionString));
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, mysqlServerVersion));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        // Configuração JWT
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        return services;
    }
}