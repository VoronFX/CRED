using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace CRED2.Model.DTOs
{
    public sealed class HistorySyncRequest
    {
        [Required]
        public KeyValuePair<long, long>[] BranchVersions { get; set; }

        public long LastChangeId { get; set; }

        public long LastCommitId { get; set; }

        public long LastKeyId { get; set; }

    }
}
