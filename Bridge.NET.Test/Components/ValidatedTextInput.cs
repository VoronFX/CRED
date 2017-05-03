using System;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components
{
	public class ValidatedTextInput : PureComponent<ValidatedTextInput.Props>
	{
		public ValidatedTextInput(
			bool disabled,
			string content,
			Action<string> onChange,
			Optional<NonBlankTrimmedString> validationMessage = new Optional<NonBlankTrimmedString>(),
			Optional<NonBlankTrimmedString> className = new Optional<NonBlankTrimmedString>())
			: base(new Props(className, disabled, content, onChange, validationMessage)) { }

		public override ReactElement Render()
		{
			var className = props.ClassName.IsDefined ? props.ClassName.Value : "";
			ReactElement validationMessageIfAny;
			if (props.ValidationMessage.IsDefined)
			{
				className += (className == "" ? "" : " ") + "invalid";
				validationMessageIfAny = DOM.Span(new Attributes { ClassName = "validation-message" }, props.ValidationMessage.Value);
			}
			else
				validationMessageIfAny = null;

			return DOM.Span(new Attributes { ClassName = className },
				new TextInput(props.Disabled, props.Content, props.OnChange, props.ClassName),
				validationMessageIfAny
			);
		}

		public class Props : TextInput.Props, IAmImmutable
		{
			public Props(Optional<NonBlankTrimmedString> className, bool disabled, string content, Action<string> onChange, Optional<NonBlankTrimmedString> validationMessage)
				: base(className, disabled, content, onChange)
			{
				this.CtorSet(_ => _.ValidationMessage, validationMessage);
			}
			public Optional<NonBlankTrimmedString> ValidationMessage { get; }
		}
	}
}
