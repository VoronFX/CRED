namespace Bridge.NET.Test.Components.Azure
{
	public static class FxSymbol
	{
		public static string ToElementId(this FxSymbols symbol)
			=> $"FxSymbol0-{(int)symbol:x3}";
	}

	public enum FxSymbols
	{
		Hamburger = 0x012,
	}
}