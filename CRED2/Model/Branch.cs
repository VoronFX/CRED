﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRED2.Model
{
    public sealed class Branch
    {
		public string Name { get; set; }

		public long CommitId { get; set; }

		public DateTime LastUpdated { get; set; }

		public bool Broken { get; set; }

		public string Message { get; set; }

		public bool GitBranch { get; set; }

		public string GitRemoteUrl { get; set; }

		public string GitRemoteRef { get; set; }

		public string GitUsername { get; set; }

		public string GitPassword { get; set; }

    }

}
