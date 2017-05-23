using AzurePortal;
using Bridge.React;
using CRED.Client.Components.Azure.Resources;
using ProductiveRage.Immutable;

namespace CRED.Client.Components.Azure
{
	public class AvatarMenu : PureComponent<AvatarMenu.Props>
	{
		public AvatarMenu(Fxs fxs)
			:base(new Props(fxs))
		{}

		public override ReactElement Render()
		{
			return null;
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