using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Infrastructure.Mail;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;
using System;

namespace Ordering.Infrastructure
{
	public static class InfrastructureServiceRegistration
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<OrderContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("OrderingConnectionString")));

			services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
			services.AddScoped<IOrderRepository, OrderRepository>();

			services.Configure<EmailSettings>(options =>
			{
				int port;
				options.Host = configuration["EmailSettings:Host"];
				if (int.TryParse(configuration["EmailSettings:Port"], out port))
				{
					options.Port = port;
				} else
				{
					options.Port = 0;
				}
				options.User = configuration["EmailSettings:User"];
				options.Password = configuration["EmailSettings:Password"];
			});
			services.AddTransient<IEmailService, EmailService>();

			return services;
		}
	}
}
