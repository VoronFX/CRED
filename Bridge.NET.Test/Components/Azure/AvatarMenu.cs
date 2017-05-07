using AzurePortal;
using Bridge.NET.Test.Components.Azure.Resources;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public class AvatarMenu : PureComponent<AvatarMenu.Props>
	{
		public AvatarMenu(Fxs fxs)
			:base(new Props(fxs))
		{}

		public override ReactElement Render()
		{
			throw new System.NotImplementedException();
		}

		private StyleClassesMap Classes => props.Fxs.StyleClasses;
		private DummyClassesMap DummyClasses => props.Fxs.DummyClasses;
		private IFxsText Text => props.Fxs.Text;

		public sealed class Props : IAmImmutable
		{
			public Props(Fxs fxs)
			{
				this.CtorSet(_ => _.Fxs, fxs);
			}

			public Fxs Fxs { get; }
		}
	}
}