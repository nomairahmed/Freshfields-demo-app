using System;
using ExerciseApp.Data;
using ExerciseApp.Service;
using ExerciseApp.Service.Pricing;
using ExerciseApp.Service.Rules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

namespace ExerciseApp
{
    public class Startup(IConfiguration configuration)
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
                    option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
            services.AddCors();

            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<IBasePriceProvider, PriceTable>();
            services.AddSingleton<IQuoteRule, AgeEligibilityRule>();
            services.AddSingleton<QuoteService>();
            services.AddSingleton<VehicleCatalogue>();

            services.AddDbContext<QuoteDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("Quotes")));
            services.AddScoped<IQuoteRepository, EfQuoteRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<QuoteDbContext>().Database.Migrate();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins("*");
                builder.WithMethods("GET", "PUT", "POST", "DELETE", "HEAD");
                builder.WithHeaders("Origin", "X-Requested-With", "content-type", "Accept");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
