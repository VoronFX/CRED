using AzurePortal;

namespace Bridge.NET.Test.Components.Azure.Resources
{
	public sealed partial class Fxs
	{
		public StyleClassesMap StyleClasses { get; } = new StyleClassesMap();
		public DummyClassesMap DummyClasses { get; } = new DummyClassesMap();
		public IFxsText Text { get; } = new FxsTextRu();


	}
}
