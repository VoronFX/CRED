using System.Collections.Generic;

namespace CRED2.Model
{
	public sealed class Key
	{
		public static IEqualityComparer<Key> PathKeyPartsComparer { get; } = new PathKeyPartsEqualityComparer();

		public long Id { get; set; }

		public string[] Path { get; set; }

		public string[] KeyParts { get; set; }

		private sealed class PathKeyPartsEqualityComparer : IEqualityComparer<Key>
		{
			public bool Equals(Key x, Key y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return Equals(x.Path, y.Path) && Equals(x.KeyParts, y.KeyParts);
			}

			public int GetHashCode(Key obj)
			{
				unchecked
				{
					return ((obj.Path != null ? obj.Path.GetHashCode() : 0) * 397) ^
					       (obj.KeyParts != null ? obj.KeyParts.GetHashCode() : 0);
				}
			}
		}
	}
}