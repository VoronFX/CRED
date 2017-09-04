using System;
using System.Collections.Generic;

namespace CRED2.Model
{
	public sealed class HistoryItem
	{
		public static IEqualityComparer<HistoryItem> HistoryItemComparer { get; } = new HistoryItemEqualityComparer();

		public long Id { get; set; }

		public HistoryItem Parent { get; set; }

		public Change Change { get; set; }

		public string GitCommitHash { get; set; }

		public string Comitter { get; set; }

		public DateTime Timestamp { get; set; }

		private sealed class HistoryItemEqualityComparer : IEqualityComparer<HistoryItem>
		{
			public bool Equals(HistoryItem x, HistoryItem y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return Equals(x.Change, y.Change) && string.Equals(x.GitCommitHash, y.GitCommitHash) &&
				       string.Equals(x.Comitter, y.Comitter) && x.Timestamp.Equals(y.Timestamp);
			}

			public int GetHashCode(HistoryItem obj)
			{
				unchecked
				{
					int hashCode = obj.Change != null ? obj.Change.GetHashCode() : 0;
					hashCode = (hashCode * 397) ^ (obj.GitCommitHash != null ? obj.GitCommitHash.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj.Comitter != null ? obj.Comitter.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ obj.Timestamp.GetHashCode();
					return hashCode;
				}
			}
		}
	}
}