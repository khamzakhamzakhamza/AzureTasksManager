using AzureTasksManagerAPI.DataAccess;
using AzureTasksManagerAPI.DataAccess.Repository;
using AzureTasksManagerAPI.DataAccess.Repository.Interfaces;
using AzureTasksManagerAPI.Hubs;
using AzureTasksManagerAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureTasksManagerAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHostedService<QueueServices>();
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });
            services.AddAzureClients(builder =>
            {
                builder.AddQueueServiceClient(this.Configuration["Queue:ConnectionString"], preferMsi: true);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TasksHub>("/hubs/tasks");
            });
        }
    }
}
