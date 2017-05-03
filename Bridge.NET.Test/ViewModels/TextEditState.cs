using ProductiveRage.Immutable;

namespace Bridge.NET.Test.ViewModels
{
	public class TextEditState : IAmImmutable
	{
		public TextEditState(string text) : this(text, Optional<NonBlankTrimmedString>.Missing) { }
		public TextEditState(string text, Optional<NonBlankTrimmedString> validationError)
		{
			this.CtorSet(_ => _.Text, text);
			this.CtorSet(_ => _.ValidationError, validationError);
		}
		public string Text { get; }
		public Optional<NonBlankTrimmedString> ValidationError { get; }
	}
}
