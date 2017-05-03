using System;
using System.Linq;
using Bridge.NET.Test.Helpers;
using Bridge.NET.Test.ViewModels;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public class Portal : PureComponent<Portal.Props>
	{
		public Portal(NonNullList<IStyleClass> classNames, PortalTheme theme)
			: base(new Props(classNames, theme)) { }

		private enum FxsContainerClasses { FxsPortal, FxsDesktopNormal, FxsShowStartboard }

		public override ReactElement Render()
		{
			//fxs - portal fxs - desktop - normal fxs - show - startboard
			//"<div id="web - container" class="fxs - portal fxs - desktop - normal fxs - show - journey">
			//	< div class="fxs-topbar">
			//	</div>
			//	<div class="fxs-portal-tip"></div>
			//	<div class="fxs-portal-main">
			//	<div class="fxs-sidebar fxs-trim"></div>
			//	<div class="portal-contextpane" alignment="left"></div>
			//	<div class="fxs-portal-content fxs-scrollbar-transparent fxs-scrollbar-default-hover fxs-panorama"></div>
			//	<div class="portal-contextpane" alignment="right"></div>
			//	</div>
			//	<azure-svg-symbols></azure-svg-symbols>
			//	</div>
			//	"
			return DOM.Div(new Attributes
			{
				Id = "container",
				ClassName = props.ClassNames.Union<IStyleClass>(new []{ 
					FxsContainerClasses.FxsPortal,
					FxsContainerClasses.FxsDesktopNormal, 
					FxsContainerClasses.FxsShowStartboard})
					.ToClassesString()
			}

			);


			var formIsInvalid = props.Message.Title.ValidationError.IsDefined || props.Message.Content.ValidationError.IsDefined;
			var isSaveDisabled = formIsInvalid || props.Message.IsSaveInProgress;
			return DOM.Div(new FieldSetAttributes { ClassName = props.ClassName.IsDefined ? props.ClassName.Value : null },
				DOM.Legend(null, props.Message.Caption.Value),
				DOM.Span(new Attributes { ClassName = "label" }, "Title"),
				new ValidatedTextInput(
					className: new NonBlankTrimmedString("title"),
					disabled: props.Message.IsSaveInProgress,
					content: props.Message.Title.Text,
					validationMessage: props.Message.Title.ValidationError,
					onChange: newTitle => props.OnChange(props.Message.With(_ => _.Title, new TextEditState(newTitle)))
				),
				DOM.Span(new Attributes { ClassName = "label" }, "Content"),
				new ValidatedTextInput(
					className: new NonBlankTrimmedString("content"),
					disabled: props.Message.IsSaveInProgress,
					content: props.Message.Content.Text,
					validationMessage: props.Message.Content.ValidationError,
					onChange: newContent => props.OnChange(props.Message.With(_ => _.Content, new TextEditState(newContent)))
				),
				DOM.Button(
					new ButtonAttributes { Disabled = isSaveDisabled, OnClick = e => props.OnSave() },
					"Save"
				)
			);
		}

		public class Props : IAmImmutable
		{
			public Props(NonNullList<IStyleClass> classNames, PortalTheme theme)
			{
				this.CtorSet(_ => _.ClassNames, classNames);
				this.CtorSet(_ => _.Theme, theme);
			}
			public NonNullList<IStyleClass> ClassNames { get; }
			public PortalTheme Theme { get; }
		}

		public enum PortalTheme
		{
			Azure, Blue, Light, Black
		}
	}
}
