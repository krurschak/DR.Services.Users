using DR.Frameworks.Users.Models;
using DR.Packages.MassTransit;
using DR.Packages.Mongo;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DR.Services.Users
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add health checks
            services.AddHealthChecks();
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = (check) => check.Tags.Contains("ready");
            });

            // register MongoDBContext
            services.AddMongoContext(Configuration.GetConnectionString("DefaultConnection"), cfg =>
            {
                cfg.AddMongoRepository<Event>("Events");
                cfg.AddMongoRepository<User>("Users");
            });

            services
                .AddMassTransit(cfg =>
                {
                    cfg.AddConsumers(Assembly.GetExecutingAssembly());
                    cfg.UseRabbitMq(Configuration.GetConnectionString("RabbitMq"), Assembly.GetExecutingAssembly());
                })
                .AddMassTransitHostedService();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions());

                endpoints.MapControllers();
            });
        }
    }
}
