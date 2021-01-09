using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureTasksManagerAPI.DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AzureTasksManagerAPI.Services;
using Microsoft.Extensions.Configuration;
using AzureTasksManagerSDK.Entities;

namespace AzureTasksManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public TasksController(IConfiguration config, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<AzureTask>>> GetAllTasks()
        {
            try
            {
                return this.Ok(await this.unitOfWork.Tasks.GetAll());
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("Create/{name}")]
        public async Task<ActionResult<AzureTask>> CreateTasks(string name)
        {
            try
            {
                var task = new AzureTask(name);
                this.unitOfWork.Tasks.Add(task);
                await this.unitOfWork.Save();
                await QueueServices.SendTask(task);
                return this.Ok(task);
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, ex.Message);
            }
        }
    }
}