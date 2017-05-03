using System;
using Bridge.NET.Test.ViewModels;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components
{
	public class MessageEditor : PureComponent<MessageEditor.Props>
	{
		public MessageEditor(Optional<NonBlankTrimmedString> className, MessageEditState message, Action<MessageEditState> onChange, Action onSave)
			: base(new Props(className, message, onChange, onSave)) { }

		public override ReactElement Render()
		{
			var formIsInvalid = props.Message.Title.ValidationError.IsDefined || props.Message.Content.ValidationError.IsDefined;
			var isSaveDisabled = formIsInvalid || props.Message.IsSaveInProgress;
			return DOM.FieldSet(new FieldSetAttributes { ClassName = props.ClassName.IsDefined ? props.ClassName.Value : null },
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
			public Props(Optional<NonBlankTrimmedString> className, MessageEditState message, Action<MessageEditState> onChange, Action onSave)
			{
				this.CtorSet(_ => _.ClassName, className);
				this.CtorSet(_ => _.Message, message);
				this.CtorSet(_ => _.OnChange, onChange);
				this.CtorSet(_ => _.OnSave, onSave);
			}
			public Optional<NonBlankTrimmedString> ClassName { get; private set; }
			public MessageEditState Message { get; private set; }
			public Action<MessageEditState> OnChange { get; private set; }
			public Action OnSave { get; private set; }
		}
	}
}
