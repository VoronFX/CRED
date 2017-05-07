using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bridge.React;
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

		//public static TAttr AddClass<TAttr>(this TAttr attributes, Func<bool> condition, params string[] classes) 
		//	where TAttr : DomElementsAttributes
		//{
		//	attributes.ClassName.Join(" ", attributes.ClassName.Split(" ").Join(classes, s => s, s => s));
		//	return attributes;
		//}
	}

	public static class Fluent
	{

		public sealed class FluentList<T> : List<T>
		{
			public new FluentList<T> Add(T item) => this.FluentAdd(item);
			public new FluentList<T> AddRange(IEnumerable<T> item) => this.FluentAddRange(item);
			public FluentList<T> DistinctAdd(T item) => this.FluentDistinctAdd(item);
			public FluentList<T> DistinctAddRange(IEnumerable<T> item) => this.FluentDistinctAddRange(item);
		}

		public sealed class FluentClassName : FluentList<string>
		{
			public FluentClassName AddIf(Func<bool> condition, params string[] classNames)
				=> condition() ? AddRange(classNames) : this;

			public FluentClassName Add(params string[] classNames)
			{
				AddRange(classNames);
				return this;
			}

			public static implicit operator string(FluentClassName obj)
				=> string.Join(" ", obj.Distinct());
		}

		public static FluentClassName ClassName(params string[] classNames)
			=> new FluentClassName().Add(classNames);

		public static T Append<T, T2>(this T list, T2 item) where T : IList<T2>
		{
			list.Add(item);
			return list;
		}

		public static T AppendIf<T, T2>(this T list, T2 item) where T : IList<T2>
		{
			list.Add(item);
			return list;
		}

		public static FluentList<T> List<T>()
			=> new FluentList<T>();

		public static FluentList<T> ToFuentList<T>(this T list) where T : IList<T>
			=> new FluentList<T>().AddRange(list);

		public static T FluentAdd<T, T2>(this T collection, T2 item) where T : ICollection<T2>
		{
			if (!collection.Contains(item))
				collection.Add(item);
			return collection;
		}

		public static T FluentDistinctAdd<T, T2>(this T collection, T2 item) where T : ICollection<T2>
		{
			if (!collection.Contains(item))
				collection.Add(item);
			return collection;
		}

		public static T FluentDistinctAddRange<T, T2>(this T collection, IEnumerable<T2> items) where T : ICollection<T2>
			=> collection.FluentAddRange(items.Except(collection));

		public static T FluentAddRange<T, T2>(this T collection, IEnumerable<T2> items) where T : ICollection<T2>
		{
			foreach (var item in items)
			{
				collection.Add(item);
			}
			return collection;
		}

		public static T FluentIf<T>(this T obj, Func<T, bool> condition, Func<T, T> action)
			=> condition(obj) ? action(obj) : obj;

		public static T FluentIf<T>(this T obj, Func<T, bool> condition, Action<T> action)
		{
			if (condition(obj))
				action(obj);
			return obj;
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