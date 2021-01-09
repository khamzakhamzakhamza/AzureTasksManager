using System.Text.Json;
using System.Threading.Tasks;
using AzureTasksManagerAPI.DataAccess.Repository.Interfaces;
using AzureTasksManagerSDK.Entities;
using Microsoft.Azure.WebJobs;
using System;

namespace AzureTasksManagement.Functions
{
    public class UpdateFunction
    {
        private readonly IUnitOfWork unitOfWork;

        public UpdateFunction(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task ProcessQueueMessage([QueueTrigger("updated-tasks")] string message)
        {
            AzureTask task = JsonSerializer.Deserialize<AzureTask>(message);    
            this.unitOfWork.Tasks.Update(task);    
            await this.unitOfWork.Save();
            if (task.Finished == true)
            {
                while (DateTime.Now.Minute - task.FinishedTime.Minute < 1)
                {
                    // Wait for a minute
                }

                this.unitOfWork.Tasks.Remove(task);
                await this.unitOfWork.Save();
            }
        }
    }
}