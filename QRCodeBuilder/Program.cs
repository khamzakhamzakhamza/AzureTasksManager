using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace QRCodeBuilder
{
    class Program
    {
        public static async Task Main()
        {
            HostBuilder hostBuilder = new HostBuilder();
            hostBuilder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorageQueues();
            });
            using (IHost host = hostBuilder.Build())
            {
                await host.RunAsync();
            }
        }
    }
}
