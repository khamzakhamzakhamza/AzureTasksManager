using AzureTasksManagerAPI.Hubs.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using AzureTasksManagerSDK.Entities;

namespace AzureTasksManagerAPI.Hubs
{
    public class TasksHub : Hub<ITasksHub>
    {
        public void BrodcastTask(string taskJson)
        {
            Clients.All.UpdateTask(JsonSerializer.Deserialize<AzureTask>(taskJson));
        }
    }
}