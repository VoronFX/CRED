using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CRED2.Data;
using CRED2.Helpers;
using CRED2.Model;
using CRED2.Model.DTOs;

using Microsoft.AspNetCore.Mvc;

namespace CRED2.Api
{
    [ValidateModel]
    public class HistoryController : Controller
    {
        public HistoryController(HistoryRepository database)
        {
            this.Database = database;
        }

        public HistoryRepository Database { get; }

        [HttpPost]
        public async Task<IActionResult> Sync([FromBody] HistorySyncRequest request, CancellationToken cancellationToken)
        {
            var branches = this.Database.Fetch<Branch>();

            var commits = Task.Run(
                () => this.Database.Fetch<Commit>(x => x.Id > request.LastCommitId).ToArray(),
                cancellationToken);
            var changes = Task.Run(
                () => this.Database.Fetch<Change>(x => x.Id > request.LastChangeId).ToArray(),
                cancellationToken);
            var keys = Task.Run(
                () => this.Database.Fetch<Key>(x => x.Id > request.LastKeyId).ToArray(),
                cancellationToken);

            var response = new HistorySyncResponse
                               {
                                   RemovedBranchesIds =
                                       request.BranchVersions.Select(x => x.Key)
                                           .Except(branches.Select(x => x.Id)).ToArray(),
                                   ChangedBranches =
                                       branches.Where(
                                               x => request.BranchVersions.All(
                                                   x2 => x2.Key != x.Id && x2.Value != x.RawVersion))
                                           .ToArray(),
                                   Commits = await commits,
                                   Changes = await changes,
                                   Keys = await keys,
                               };

            return this.Ok(response);
        }
    }
}