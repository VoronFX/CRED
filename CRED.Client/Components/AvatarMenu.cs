using System.Linq;
using Bridge.Html5;
using Bridge.React;
using CRED.Client.API;
using CRED.Client.Helpers;
using CRED.Client.TypedMaps;
using ProductiveRage.Immutable;
using NonBlankTrimmedString = ProductiveRage.Immutable.NonBlankTrimmedString;

namespace CRED.Client.Components
{
	public sealed class AvatarMenu : PureComponent<AvatarMenu.Props>
	{
		public AvatarMenu()
			: base(new Props())
		{
		}

		public override ReactElement Render()
		{
			return DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Styles.NavbarItem, Styles.HasDropdown, Styles.IsHoverable)
				},
				DOM.A(new AnchorAttributes
					{
						ClassName = Fluent.ClassName(Styles.NavbarItem),
						Href = "/"
					},
					DOM.I(new Attributes
						{
							ClassName = Fluent.ClassName(Styles.MaterialIcons, Styles.Md36)
						},
						MaterialIcons.AccountCircle
					),
					"Exit"
				),
				DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Styles.NavbarDropdown, Styles.IsRight)
					},
					DOM.A(new AnchorAttributes
						{
							ClassName = Fluent.ClassName(Styles.NavbarItem),
							Href = "/"
						},
						"Exit"
					)
				)
			);
		}

		public class Props : IAmImmutable
		{
			public Props()
			{
				//this.CtorSet(_ => _.ClassName, className);
				//this.CtorSet(_ => _.Messages, messages);
			}

			//public Optional<NonBlankTrimmedString> ClassName { get; }
			//public NonNullList<SavedMessageDetails> Messages { get; }
		}
	}
}