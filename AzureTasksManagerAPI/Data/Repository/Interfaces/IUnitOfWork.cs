using System;
using System.Threading.Tasks;
using AzureTasksManagerSDK.Entities;

namespace AzureTasksManagerAPI.DataAccess.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<AzureTask> Tasks { get; }

        Task<int> Save();
    }
}