using AzureTasksManagerSDK.Entities;
using System.Threading.Tasks;

namespace AzureTasksManagerAPI.Hubs.Interfaces
{
    public interface ITasksHub
    {
        Task UpdateTask(AzureTask task);
    }
}