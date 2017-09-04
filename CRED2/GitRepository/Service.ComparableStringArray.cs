using System;
using System.Collections.Generic;
using System.Linq;

namespace CRED2.GitRepository
{
	public struct ComparableStringArray : IEquatable<ComparableStringArray>
	{
		private sealed class StringArrayEqualityComparer : IEqualityComparer<string[]>
		{
			public bool Equals(string[] x, string[] y)
			{
				if (x == null && y == null)
					return true;
				if (x == null || y == null)
					return false;
				if (x.Length != y.Length)
					return false;
				return !x.Where((t, i) => !t.Equals(y[i], StringComparison.Ordinal)).Any();
			}

			public int GetHashCode(string[] obj)
			{
				return EqualityComparer<string[]>.Default.GetHashCode(obj);
			}
		}

		public static IEqualityComparer<string[]> EqualityComparer { get; } = new StringArrayEqualityComparer();

		public ComparableStringArray(string[] array)
		{
			Array = array;
		}

		private string[] Array { get; }

		public override bool Equals(object obj)
		{
			return obj is ComparableStringArray && Equals((ComparableStringArray)obj);
		}

		public bool Equals(ComparableStringArray other)
		{
			return EqualityComparer.Equals(Array, other.Array);
		}

		public override int GetHashCode()
		{
			var hashCode = 382270662;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(Array);
			return hashCode;
		}

		public static bool operator ==(ComparableStringArray x, ComparableStringArray y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(ComparableStringArray x, ComparableStringArray y)
		{
			return !(x == y);
		}

		public static implicit operator string[] (ComparableStringArray x)
		{
			return x.Array;
		}

		public static implicit operator ComparableStringArray(string[] x)
		{
			return new ComparableStringArray(x);
		}
	}
}