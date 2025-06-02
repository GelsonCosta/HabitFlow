using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HabitFlow.Application;

  public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }

