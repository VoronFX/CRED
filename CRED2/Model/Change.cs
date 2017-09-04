using System;
using System.Collections.Generic;

namespace CRED2.Model
{
	public sealed class Change
	{
		public long Id { get; set; }

		public string Author { get; set; }

		public DateTime Timestamp { get; set; }

		public Key Key { get; set; }

		public string Value { get; set; }

		public static IEqualityComparer<Change> ChangeComparer { get; } = new ChangeEqualityComparer();

		private sealed class ChangeEqualityComparer : IEqualityComparer<Change>
		{
			public bool Equals(Change x, Change y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return string.Equals(x.Author, y.Author) && x.Timestamp.Equals(y.Timestamp) && Equals(x.Key, y.Key) &&
				       string.Equals(x.Value, y.Value);
			}

			public int GetHashCode(Change obj)
			{
				unchecked
				{
					int hashCode = obj.Author != null ? obj.Author.GetHashCode() : 0;
					hashCode = (hashCode * 397) ^ obj.Timestamp.GetHashCode();
					hashCode = (hashCode * 397) ^ (obj.Key != null ? obj.Key.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (obj.Value != null ? obj.Value.GetHashCode() : 0);
					return hashCode;
				}
			}
		}
	}
}