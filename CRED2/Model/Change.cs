using System.Collections.Generic;

namespace CRED2.Model
{
    public sealed class Change
    {
        public static IEqualityComparer<Change> KeyIdComparer { get; } = new KeyIdEqualityComparer();

        public long CommitId { get; set; }

        public long Id { get; set; }

        public long KeyId { get; set; }

        // public Key Key { get; set; }

        public string Value { get; set; }

        private sealed class KeyIdEqualityComparer : IEqualityComparer<Change>
        {
            public bool Equals(Change x, Change y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.KeyId == y.KeyId;
            }

            public int GetHashCode(Change obj)
            {
                return obj.KeyId.GetHashCode();
            }
        }
    }
}