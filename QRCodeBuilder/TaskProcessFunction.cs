using System;
using System.Drawing;
using System.Threading.Tasks;
using AzureTasksManagerSDK.Entities;
using AzureTasksManagerSDK.Enum;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System.Text.Json;
using System.Drawing.Imaging;
using Azure.Storage.Blobs;
using System.IO;
using Azure.Storage.Queues;
using Microsoft.AspNet.SignalR.Client;

namespace QRCodeBuilder
{
    public class QRBuilderFunction
    {
        private readonly IConfiguration config;
        private readonly HubConnection hub;
        private readonly IHubProxy hubProxy;
        private const string host = "https://localhost:44353/";

        public QRBuilderFunction(IConfiguration config)
        {
            this.config = config;
            this.hub = new HubConnection($"{host}hubs/tasks");
            this.hubProxy = this.hub.CreateHubProxy("TasksHub");
        }

        public async Task ProcessQueueMessage([QueueTrigger("tasks")] string message)
        {
            try
            {
                if (this.hub.State == ConnectionState.Disconnected)
                {
                    await this.hub.Start();
                }

                AzureTask task = JsonSerializer.Deserialize<AzureTask>(message);
                task.Duration = this.GetRandomNumber(5, 15);
                task.Status = EnumStatus.Success;
                task.Finished = true;
                task.QRCode = await this.CreateQRCode();
                string updatedTaskJson = JsonSerializer.Serialize(task);
                //Sending tasks to queue
                QueueClient queueClient = new QueueClient(this.config["AzureWebJobsStorage"], this.config["Queue:ResultQueueName"]);
                await queueClient.CreateIfNotExistsAsync();
                await queueClient.SendMessageAsync(updatedTaskJson);
                Console.WriteLine($"Task id:{task.Id} name:{task.Name} proccesed");

                await this.hubProxy.Invoke("BrodcastTask", updatedTaskJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #region Misc

        private int GetRandomNumber(int from, int to) => new Random().Next(from, to);

        private bool InPercentageMargin(int percentage) => new Random().Next(1, 101) <= percentage;

        #endregion

        #region QRCode

        private async Task<string> CreateQRCode()
        {
            string text = Guid.NewGuid().ToString();
            QRCodeGenerator qrCodeGen = new QRCodeGenerator();
            QRCodeData qrCodeData = qrCodeGen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            string qrCodeImagePath = text + ".png";
            await this.PostImageToBlob(qrCodeImage, qrCodeImagePath);
            return qrCodeImagePath;
        }

        private async Task PostImageToBlob(Bitmap img, string path)
        {
            img.Save(path, ImageFormat.Png);
            BlobClient blobClient = new BlobClient(this.config["AzureWebJobsStorage"], this.config["BlobStorName"], path);
            if (!await blobClient.ExistsAsync())
            {
                await blobClient.UploadAsync(path);
            }

            File.Delete(path);
            img.Dispose();
        }

        #endregion
    }
}