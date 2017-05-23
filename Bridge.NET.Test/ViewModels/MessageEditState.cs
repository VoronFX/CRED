using ProductiveRage.Immutable;

namespace CRED.Client.ViewModels
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
		public NonBlankTrimmedString Caption { get; }
		public TextEditState Title { get; }
		public TextEditState Content { get; }
		public bool IsSaveInProgress { get; }
	}
}
