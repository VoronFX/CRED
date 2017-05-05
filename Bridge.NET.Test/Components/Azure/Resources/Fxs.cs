namespace Bridge.NET.Test.Components.Azure.Resources
{
	public sealed partial class Fxs
	{
		public FxsClasses Classes { get; } = new FxsClasses();
		public IFxsText Text { get; } = new FxsTextRu();


	}
}
