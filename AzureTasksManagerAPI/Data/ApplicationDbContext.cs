using Microsoft.EntityFrameworkCore;
using AzureTasksManagerSDK.Entities;

namespace AzureTasksManagerAPI.DataAccess
{
    /// <summary>
    /// Db context for Entity Framework.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">Connection options.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<AzureTask> Tasks { get; set; }
    }
}