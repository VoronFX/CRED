using System;
using Bridge.Html5;
using Bridge.React;
using ProductiveRage.Immutable;

namespace CRED.Client.Components
{
	public class TextInput : PureComponent<TextInput.Props>
	{
		public TextInput(bool disabled, string content, Action<string> onChange, Optional<NonBlankTrimmedString> className = new Optional<NonBlankTrimmedString>())
			: base(new Props(className, disabled, content, onChange)) { }

		public override ReactElement Render()
		{
			return DOM.Input(new InputAttributes
			{
				Type = InputType.Text,
				ClassName = props.ClassName.IsDefined ? props.ClassName.Value : null,
				Disabled = props.Disabled,
				Value = props.Content,
				OnChange = e => props.OnChange(e.CurrentTarget.Value)
			});
		}

		public class Props : IAmImmutable
		{
			public Props(Optional<NonBlankTrimmedString> className, bool disabled, string content, Action<string> onChange)
			{
				this.CtorSet(_ => _.ClassName, className);
				this.CtorSet(_ => _.Disabled, disabled);
				this.CtorSet(_ => _.Content, content);
				this.CtorSet(_ => _.OnChange, onChange);
			}
			public Optional<NonBlankTrimmedString> ClassName { get; }
			public bool Disabled { get; }
			public string Content { get; }
			public Action<string> OnChange { get; }
		}
	}
}
