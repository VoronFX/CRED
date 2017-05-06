using System;
using Bridge.Html5;
//using CRED;
//using static AzurePortal.Definitions.Svg;

namespace Bridge.NET.Test.Components.Azure.Resources
{
	public static class FxSymbolExtension
	{
		static FxSymbolExtension()
		{
			foreach (Fxs.Symbols value in Enum.GetValues(typeof(Fxs.Symbols)))
			{
				if (Document.GetElementById(value.ToElementId()) == null)
				{
					throw new ArgumentException($"Resource {value.ToHref()} was not found.");
				}
			}
		}

		public static string ToElementId(this Fxs.Symbols symbol)
			=> $"FxSymbol0-{(int)symbol:x3}";

		public static string ToHref(this Fxs.Symbols symbol)
			=> $"#{symbol.ToElementId()}";
	}

	//[RequireResource(Svg_FxSymbol0_00f_svg)]
	//[RequireResource(Svg_FxSymbol0_010_svg)]
	//[RequireResource(Svg_FxSymbol0_013_svg)]
	//[RequireResource(Svg_FxSymbol0_011_svg)]
	//[RequireResource(Svg_FxSymbol0_00e_svg)]
	public sealed partial class Fxs
	{
		public enum Symbols
		{
			Hamburger = 0x00f,
			Plus = 0x010,
			Ellipsis = 0x013,
			CaretUp = 0x011,
			Search = 0x00e
		}
	}
}