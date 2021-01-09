using AzureTasksManagerSDK.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace AzureTasksManagerSDK.Entities
{
    public class AzureTask
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime CreationDate { get; private set; }

        public string QRCode { get; set; }

        public EnumStatus Status { get; set; }

        public EnumStatus ResultStatus { get; set; }

        public bool Finished { get; set; }

        public int ReadyPercents { get; set; }

        public int Duration { get; set; }

        public DateTime FinishedTime { get; set; }

        public AzureTask()
        {
        }

        public AzureTask(string name)
        {
            this.Name = name;
            this.CreationDate = DateTime.Now;
            this.Status = EnumStatus.Inprogress;
        }
    }
}
