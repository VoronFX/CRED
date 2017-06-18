using Bridge.React;
using CRED.Client.Components.Azure.Resources;
using ProductiveRage.Immutable;
using Classes = CRED.Client.AzureCssClassesMap;
using DummyClasses = CRED.Client.AzureCssMissingClassesMap;

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