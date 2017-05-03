using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Helpers
{
	public static class StyleClassExtensionMethods
	{
		public static string ToClassesString<T>(this IEnumerable<T> clasessList) where T : IStyleClass
			=> string.Join(" ", clasessList.Select(x => x.ToString()));

		public static string ToCssClassName(this string name)
			=> ToCssClassName(name, StyleClassSeparator.Hyphen);

		public static string ToCssClassName(this string name, StyleClassSeparator separator)
		{
			name = String.Join(GetSeparatorChar(separator),
				Regex.Replace(Regex.Replace(name, "[^a-zA-Z_-]", ""), "([a-zA-Z])(?=[A-Z])", "$1-"))
				.ToLower();
			if (name.Length < 2)
				throw new ArgumentException($"Resulting class name is less than 2 symbols: \"{name}\"");
			return name;
		}

		private static string GetSeparatorChar(StyleClassSeparator separator)
		{
			switch (separator)
			{
				case StyleClassSeparator.Underscope:
					return "_";
				case StyleClassSeparator.Hyphen:
					return "-";
				case StyleClassSeparator.None:
					return "";
				default:
					throw new ArgumentOutOfRangeException(nameof(separator), separator, null);
			}
		}
	}

	public interface IStyleClass
	{
		string ToString();
		string ToString(StyleClassSeparator separator);
	}

	public enum StyleClassSeparator
	{
		Underscope,
		Hyphen,
		None
	}

	public struct StyleClass<T> : IStyleClass where T : struct, IConvertible
	{
		public StyleClass(T value)
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			Value = value;
		}

		public T Value { get; }

		public override string ToString() => ToString(StyleClassSeparator.Hyphen);

		public string ToString(StyleClassSeparator separator)
		{
			var name = Regex.Replace(Enum.GetName(typeof(T), this), "^[A-Z_a-z]", "");
			name = Regex.Split(name, "([A-Z_][a-z]+)")
				.Join(GetSeparatorChar(separator))
				.ToLower();
			if (name.Length < 2)
				throw new ArgumentException($"Resulting class name is less than 2 symbols: \"{name}\"");
			return name;
		}

		private static string GetSeparatorChar(StyleClassSeparator separator)
		{
			switch (separator)
			{
				case StyleClassSeparator.Underscope:
					return "_";
				case StyleClassSeparator.Hyphen:
					return "-";
				case StyleClassSeparator.None:
					return "";
				default:
					throw new ArgumentOutOfRangeException(nameof(separator), separator, null);
			}
		}
	}
}