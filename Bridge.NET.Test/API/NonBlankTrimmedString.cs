using System;
using Bridge.React;

namespace Bridge.NET.Test.API
{
	public sealed class NonBlankTrimmedString
	{
		public NonBlankTrimmedString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException("Null, blank or whitespace-only value specified");

			Value = value.Trim();
		}

		/// <summary>
		/// This will never be null, blank or have any leading or trailing whitespace
		/// </summary>
		public string Value { get; private set; }

		public override bool Equals(object o)
		{
			// Note: Implementing "Equals" on a sealed class is much easier than it would be otherwise because there are no worries about
			// what different behaviour or additional data any sub classes could introduce
			var otherNonBlankTrimmedString = o as NonBlankTrimmedString;
			if (otherNonBlankTrimmedString == null)
				return false;

			return otherNonBlankTrimmedString.Value == Value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		/// <summary>
		/// It's convenient to be able to pass a NonBlankTrimmedString instance as any argument that requires a string
		/// </summary>
		public static implicit operator string(NonBlankTrimmedString value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			return value.Value;
		}

		/// <summary>
		/// It's convenient to be able to pass a NonBlankTrimmedString instance as any argument that requires a ReactElement-or-string,
		/// since it's common for strings to be used as child elements within a component and a NonBlankTrimmedString is just a string
		/// </summary>
		public static implicit operator Any<ReactElement, string>(NonBlankTrimmedString value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			return value.Value;
		}
	}
}
