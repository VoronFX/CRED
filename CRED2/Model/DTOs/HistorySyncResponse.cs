﻿namespace CRED2.Model.DTOs
{
    public sealed class HistorySyncResponse
    {
        public Branch[] ChangedBranches { get; set; }

        public Change[] Changes { get; set; }

        public Commit[] Commits { get; set; }

        public Key[] Keys { get; set; }

        public long[] RemovedBranchesIds { get; set; }
    }
}