using ProductiveRage.Immutable;

namespace Bridge.NET.Test.ViewModels
{
	public class MessageEditState : IAmImmutable
	{
		public MessageEditState(NonBlankTrimmedString caption, TextEditState title, TextEditState content, bool isSaveInProgress)
		{
			this.CtorSet(_ => _.Caption, caption);
			this.CtorSet(_ => _.Title, title);
			this.CtorSet(_ => _.Content, content);
			this.CtorSet(_ => _.IsSaveInProgress, isSaveInProgress);
		}
		public NonBlankTrimmedString Caption { get; private set; }
		public TextEditState Title { get; private set; }
		public TextEditState Content { get; private set; }
		public bool IsSaveInProgress { get; private set; }
	}
}
