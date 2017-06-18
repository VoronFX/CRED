using System;
using System.Collections.Generic;
using System.Linq;

namespace CRED.Client.Helpers
{
	public static class Fluent
	{
		public class FluentList<T> : List<T>
		{
			public new FluentList<T> Add(T item) => this.FluentAdd(item);
			public new FluentList<T> AddRange(IEnumerable<T> item) => this.FluentAddRange(item);
			public FluentList<T> DistinctAdd(T item) => this.FluentDistinctAdd(item);
			public FluentList<T> DistinctAddRange(IEnumerable<T> item) => this.FluentDistinctAddRange(item);
		}

		public sealed class FluentClassName : FluentList<string>
		{
			public FluentClassName AddIf(Func<FluentClassName, bool> condition, params string[] classNames)
				=> this.FluentIf(condition, _ => AddRange(classNames));

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

		public static FluentList<T> ToFuentList<T>(this IEnumerable<T> items)
			=> new FluentList<T>().AddRange(items);

		public static T FluentAdd<T, T2>(this T collection, T2 item) where T : ICollection<T2>
		{
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
}