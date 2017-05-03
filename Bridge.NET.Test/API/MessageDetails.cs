using ProductiveRage.Immutable;

namespace Bridge.NET.Test.API
{
	public class MessageDetails : IAmImmutable
	{
		public MessageDetails(NonBlankTrimmedString title, NonBlankTrimmedString content)
		{
			this.CtorSet(_ => _.Title, title);
			this.CtorSet(_ => _.Content, content);
		}
		public NonBlankTrimmedString Title { get; private set; }
		public NonBlankTrimmedString Content { get; private set; }
	}
}
