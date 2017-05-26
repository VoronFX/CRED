using System;

namespace CsCodeGenerator
{
	internal static class ExtensionMethods
	{
		public static T Set<T>(this T obj, Action<T> action)
		{
			action(obj);
			return obj;
		}
	}
}