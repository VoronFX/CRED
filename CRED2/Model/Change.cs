using System;
using System.Collections.Generic;

namespace CRED2.Model
{
	public sealed class Change
	{
		public string CommitHash { get; set; }

		public Commit Commit { get; set; }

		public long KeyId { get; set; }

		public Key Key { get; set; }

		public string Value { get; set; }
	}
}