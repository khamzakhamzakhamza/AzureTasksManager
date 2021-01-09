using AzureTasksManagerAPI.DataAccess;
using AzureTasksManagerAPI.DataAccess.Repository;
using AzureTasksManagerAPI.DataAccess.Repository.Interfaces;
using AzureTasksManagerSDK.Entities;
using AzureTasksManagerAPI.Hubs;
using AzureTasksManagerAPI.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;

namespace AzureTasksManagerAPI.Services
{
    public class QueueServices : BackgroundService
    {
        private static IConfiguration config;

        public static async Task SendTask(AzureTask task)
        {
            string queueName = QueueServices.config["Queue:QueueName"];
            string taskJson = JsonSerializer.Serialize(task);
            //Sending tasks to queue
            QueueClient queueClient = new QueueClient(QueueServices.config["AzureWebJobsStorage"], queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(taskJson);
        }
        
        public QueueServices(IConfiguration config)
        {
            QueueServices.config = config;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            HostBuilder hostBuilder = new HostBuilder();
            hostBuilder
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddAzureStorageQueues();
                })
                .ConfigureServices(s =>
                {
                    s.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(QueueServices.config.GetConnectionString("DefaultConnection")));
                    s.AddScoped<IUnitOfWork, UnitOfWork>();
                });
            using (IHost host = hostBuilder.Build())
            {
                await host.RunAsync();
            }
        }

        public override void Dispose() => base.Dispose();
    }
}
