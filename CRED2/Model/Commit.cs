using System;
using System.Collections.Generic;
using LibGit2Sharp;

namespace CRED2.Model
{
	public sealed class Commit
	{
		private sealed class IdEqualityComparer : IEqualityComparer<Commit>
		{
			public bool Equals(Commit x, Commit y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return x.Id == y.Id;
			}

			public int GetHashCode(Commit obj)
			{
				return obj.Id.GetHashCode();
			}
		}

		public static IEqualityComparer<Commit> IdComparer { get; } = new IdEqualityComparer();

		public long Id { get; set; }

		public string Hash { get; set; }

		public Signature Author { get; set; }

		public Signature Committer { get; set; }

		public string Message { get; set; }

		public long[] Parents { get; set; }

		//public Change[] Changes { get; set; }

	}
}