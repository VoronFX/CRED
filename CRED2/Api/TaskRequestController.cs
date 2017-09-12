using System.Threading;
using System.Threading.Tasks;

using CRED2.Services;

using Microsoft.AspNetCore.Mvc;

namespace CRED2.Api
{
    // [ValidateModel]
    public sealed class TaskRequestController : Controller
    {
        public TaskRequestController(TaskRequestService taskRequestService)
        {
            this.TaskRequestService = taskRequestService;
        }

        private TaskRequestService TaskRequestService { get; }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            return this.Ok(await this.TaskRequestService.CreateTaskRequest(cancellationToken));
        }

        public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
        {
            return this.Ok(await this.TaskRequestService.GetTaskRequest(id, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Open(long id, [FromBody] object requestData, CancellationToken cancellationToken)
        {
            await this.TaskRequestService.OpenTaskRequest(id, requestData, cancellationToken);
            return this.Ok();
        }
    }
}