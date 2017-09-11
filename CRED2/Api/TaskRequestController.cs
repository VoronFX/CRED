using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CRED2.Model;
using CRED2.Model.DTOs;
using CRED2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CRED2
{
    //[ValidateModel]
    public sealed class TaskRequestController : Controller
    {
        private TaskRequestService TaskRequestService { get; }

        public TaskRequestController(TaskRequestService taskRequestService)
        {
            TaskRequestService = taskRequestService;
        }

        //[HttpGet("{taskRequestId}")]
        public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
        {
            return Json(await TaskRequestService.GetTaskRequest(id, cancellationToken));
        }

        //[HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            return Ok(await TaskRequestService.CreateTaskRequest(cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Open(long id, [FromBody] object requestData, CancellationToken cancellationToken)
        {
            await TaskRequestService.OpenTaskRequest(id, requestData, cancellationToken);
            return Ok();
        }

    }
}