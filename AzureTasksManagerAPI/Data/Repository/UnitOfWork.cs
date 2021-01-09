using System.Threading.Tasks;
using AzureTasksManagerAPI.DataAccess.Repository.Interfaces;
using AzureTasksManagerSDK.Entities;

namespace AzureTasksManagerAPI.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;

        public IRepository<AzureTask> Tasks { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            this.Tasks = new Repository<AzureTask>(db);
        }

        public Task<int> Save()
        {
            return this.db.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.db.Dispose();
        }
    }
}