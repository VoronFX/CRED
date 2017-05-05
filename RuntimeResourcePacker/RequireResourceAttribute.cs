using System;

namespace CRED
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public class RequireResourceAttribute : Attribute
	{
		public RequireResourceAttribute(string resourceKey, int priority = 0)
		{
			resourceKey = resourceKey.Trim();
			if (string.IsNullOrWhiteSpace(resourceKey))
				throw new ArgumentException($"{nameof(resourceKey)} should contain valid path");

			ResourceKey = resourceKey;
			Priority = priority;
		}

		public string ResourceKey { get; }
		public int Priority { get; }

		public static string ResourcesVariableName { get; } = "EmbeddedResources";
	}
}
