using System;
using Bridge.Html5;

namespace CRED.Client.Components.Azure.Resources
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

	public sealed partial class Fxs
	{
		public enum Symbols
		{
			Hamburger = 0x00f,
			Plus = 0x010,
			Ellipsis = 0x013,
			CaretUp = 0x011,
			Search = 0x00e,
			NotificationsIcon = 0x009,
			ConsoleIcon = 0x00a,
			SettingsIcon = 0x00b,
			FeedbackIcon = 0x00c,
			HelpIcon = 0x00d
		}
	}
}