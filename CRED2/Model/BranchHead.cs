using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRED2.Model
{
    public sealed class BranchHead
    {
		public string Name { get; set; }

		public Commit Head { get; set; }

		public bool GitBranch { get; set; }

		public string GitRemoteUrl { get; set; }

		public string GitRemoteRef { get; set; }

    }
}
