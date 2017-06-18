using ProductiveRage.Immutable;

namespace CRED.Client.API
{
	public interface IReadAndWriteMessages
	{
		/// <summary>
		/// A MessageSaveSucceeded action will be dispatched when this succeeds, the action's RequestId will match that returned here - after
		/// this, the API wrapper will automatically trigger a re-load and a MessageHistoryUpdated action will be dispatched once the new,
		/// expanded set of messages is available
		/// </summary>
		RequestId SaveMessage(MessageDetails message);
		
		/// <summary>
		/// A MessageHistoryUpdated action will be dispatched when this succeeds, the action's RequestId will match that returned here
		/// </summary>
		RequestId GetMessages();
	}
}
