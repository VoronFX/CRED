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

		public interface IFluentCollection<T> : ICollection<T>
		{
		}

		public interface IFluentCollection<T, T2> : IFluentCollection<T> where T2 : ICollection<T>
		{
			T2 Unwrap();
		}

		public interface IFluentList<T, T2> : IFluentCollection<T, T2>, IList<T> where T2 : IList<T>
		{

		}

		private class FluentCollectionWrapper<T, T2> : IFluentCollection<T, T2> where T2 : ICollection<T>
		{
			private readonly T2 collection;

			public FluentCollectionWrapper(T2 collection)
			{
				this.collection = collection;
			}

			public T2 Unwrap() => collection;

			public IEnumerator<T> GetEnumerator()
			{
				return collection.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable)collection).GetEnumerator();
			}

			public void Add(T item)
			{
				collection.Add(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				collection.CopyTo(array, arrayIndex);
			}

			public void Clear()
			{
				collection.Clear();
			}

			public bool Contains(T item)
			{
				return collection.Contains(item);
			}

			public bool Remove(T item)
			{
				return collection.Remove(item);
			}

			public int Count => collection.Count;

			public bool IsReadOnly => collection.IsReadOnly;
		}

		private sealed class FluentListWrapper<T, T2> : FluentCollectionWrapper<T, T2>, IFluentList<T, T2> where T2 : IList<T>
		{
			private readonly T2 list;

			public FluentListWrapper(T2 list) : base(list)
			{
				this.list = list;
			}

			public int IndexOf(T item)
			{
				return list.IndexOf(item);
			}

			public void Insert(int index, T item)
			{
				list.Insert(index, item);
			}

			public void RemoveAt(int index)
			{
				list.RemoveAt(index);
			}

			public T this[int index]
			{
				get => list[index];
				set => list[index] = value;
			}
		}

		public sealed class FluentClassName : List<string>
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

		public static IFluentList<T, List<T>> List<T>()
			=> new FluentListWrapper<T, List<T>>(new List<T>());

		public static IFluentCollection<T, T2> ToFuentCollection<T, T2>(this T2 collection) where T2 : ICollection<T>
			=> new FluentCollectionWrapper<T, T2>(collection);

		public static IFluentList<T, T2> ToFuentList<T, T2>(this T2 list) where T2 : IList<T>
		 => new FluentListWrapper<T, T2>(list);

		public static T Add<T, T2>(this T collection, T2 item) where T : IFluentCollection<T2>
		{
			if (!collection.Contains(item))
				collection.Add(item);
			return collection;
		}

		public static T DistinctAdd<T, T2>(this T collection, T2 item) where T : ICollection<T2>
		{
			if (!collection.Contains(item))
				collection.Add(item);
			return collection;
		}

		public static T DistinctAddRange<T, T2>(this T collection, IEnumerable<T2> items) where T : ICollection<T2>
			=> collection.AddRange(items.Except(collection));

		public static T AddRange<T, T2>(this T collection, IEnumerable<T2> items) where T : ICollection<T2>
		{
			foreach (var item in items)
			{
				collection.Add(item);
			}
			return collection;
		}
		//public class FluentList<T> : List<T>
		//{

		//}

		//public static FluentList<T> List<T>() => new FluentList<T>();
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