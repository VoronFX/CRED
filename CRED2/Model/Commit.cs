using System;
using System.Collections.Generic;

namespace CRED2.Model
{
	public sealed class Commit
	{
		public string Hash { get; set; }

		public List<CommitParentPair> Parents { get; set; }

		public List<CommitParentPair> Children { get; set; }

		public List<Change> Changes { get; set; }

		public string Comitter { get; set; }

		public DateTime ComitterTimestamp { get; set; }

		public string Author { get; set; }

		public DateTime AuthorTimestamp { get; set; }
	}

	public sealed class CommitParentPair
	{
		public string ChildHash { get; set; }

		public Commit Child { get; set; }

		public string ParentHash { get; set; }

		public Commit Parent { get; set; }
	}

}