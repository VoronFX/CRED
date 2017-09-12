using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRED2.GitBridge;
using CRED2.Model;
using CRED2.Model.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CRED2
{
    [ValidateModel]
    public class HistoryController : Controller
    {
        public HistoryRepository Database { get; }

        public HistoryController(HistoryRepository database)
        {
            Database = database;
        }

        [HttpPost]
        public async Task<IActionResult> Sync([FromBody] HistorySyncRequest request, CancellationToken cancellationToken)
        {
            var branches = Database.Fetch<Branch>();

            var commits = Task.Run(() => Database.Fetch<Commit>(x => x.Id > request.LastCommitId).ToArray(), cancellationToken);
            var changes = Task.Run(() => Database.Fetch<Change>(x => x.Id > request.LastChangeId).ToArray(), cancellationToken);
            var keys = Task.Run(() => Database.Fetch<Key>(x => x.Id > request.LastKeyId).ToArray(), cancellationToken);

            var response = new HistorySyncResponse
            {
                RemovedBranchesIds = request.BranchVersions
                    .Select(x => x.Key)
                    .Except(branches
                        .Select(x => x.Id))
                    .ToArray(),

                ChangedBranches = branches
                    .Where(x => request.BranchVersions.All(x2 => x2.Key != x.Id && x2.Value != x.RawVersion))
                    .ToArray(),

                Commits = await commits,
                Changes = await changes,
                Keys = await keys,
            };

            return Json(response);
        }
    }
}