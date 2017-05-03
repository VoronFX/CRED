using System;
using Bridge.Html5;
using Bridge.NET.Test.Actions;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.API
{
	/// <summary>
	/// In a real application, this would talk to the server to send and retrieve data - to keep this example simple, it handles all of the data internally
	/// (but introduces a few artificial delays to simulate the server communications)
	/// </summary>
	public class MessageApi : IReadAndWriteMessages
	{
		private readonly AppDispatcher _dispatcher;
		private Set<SavedMessageDetails> _messages;
		public MessageApi(AppDispatcher dispatcher)
		{
			if (dispatcher == null)
				throw new ArgumentNullException("dispatcher");

			_dispatcher = dispatcher;
			_messages = Set<SavedMessageDetails>.Empty;

			// To further mimic a server-based API (where other people may be recording messages of their own), after a 10s delay a periodic task will be
			// executed to retrieve a new message
			Window.SetTimeout(
				() => Window.SetInterval(GetChuckNorrisFact, 5000),
				10000
			);
		}

		/// <summary>
		/// A MessageSaveSucceeded action will be dispatched when this succeeds, the action's RequestId will match that returned here - after
		/// this, the API wrapper will automatically trigger a re-load and a MessageHistoryUpdated action will be dispatched once the new,
		/// expanded set of messages is available
		/// </summary>
		public RequestId SaveMessage(MessageDetails message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			var requestId = new RequestId();
			Window.SetTimeout( // Use SetTimeout to simulate a roundtrip to the server
				() =>
				{
					_messages = _messages.Add(new SavedMessageDetails(_messages.Count, message));
					_dispatcher.HandleServerAction(new MessageSaveSucceeded(requestId));
					Window.SetTimeout(
						() => DispatchHistoryUpdatedAction(requestId),
						500
					);
				},
				1000
			);
			return requestId;
		}

		/// <summary>
		/// A MessageHistoryUpdated action will be dispatched when this succeeds, the action's RequestId will match that returned here
		/// </summary>
		public RequestId GetMessages()
		{
			var requestId = new RequestId();
			Window.SetTimeout( // Use SetTimeout to simulate a roundtrip to the server
				() => DispatchHistoryUpdatedAction(requestId),
				1000
			);
			return requestId;
		}

		private void DispatchHistoryUpdatedAction(RequestId requestId)
		{
			// ToArray is used to return a clone of the message set - otherwise, the caller would end up with a list that is updated when the internal
			// reference within this class is updated (which sounds convenient but it's not the behaviour that would be exhibited if this was "API"
			// was really persisting messages to a server somewhere)
			_dispatcher.HandleServerAction(new MessageHistoryUpdated(requestId, _messages));
		}

		private void GetChuckNorrisFact()
		{
			var request = new XMLHttpRequest();
			request.ResponseType = XMLHttpRequestResponseType.Json;
			request.OnReadyStateChange = () =>
			{
				if (request.ReadyState != AjaxReadyState.Done)
					return;

				if ((request.Status == 200) || (request.Status == 304))
				{
					try
					{
						var apiResponse = (ChuckNorrisFactApiResponse)request.Response;
						if ((apiResponse.Type == "success") && (apiResponse.Value != null) && !string.IsNullOrWhiteSpace(apiResponse.Value.Joke))
						{
							// The Chuck Norris Facts API (http://www.icndb.com/api/) returns strings html-encoded, so they need decoding before
							// be wrapped up in a MessageDetails instance
							_messages = _messages.Add(new SavedMessageDetails(_messages.Count, new MessageDetails(
								title: new NonBlankTrimmedString("Fact"),
								content: new NonBlankTrimmedString(HtmlDecode(apiResponse.Value.Joke))
							)));
							DispatchHistoryUpdatedAction(new RequestId());
							return;
						}
					}
					catch
					{
						// Ignore any error and drop through to the fallback message-generator below
					}
				}
				_messages = _messages.Add(new SavedMessageDetails(_messages.Count, new MessageDetails(
					title: new NonBlankTrimmedString("Fact"),
					content: new NonBlankTrimmedString("API call failed when polling for server content :(")
				)));
				DispatchHistoryUpdatedAction(new RequestId());
			};
			request.Open("GET", "http://api.icndb.com/jokes/random");
			request.Send();
		}

		private string HtmlDecode(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			var wrapper = Document.CreateElement("div");
			wrapper.InnerHTML = value;
			return wrapper.TextContent;
		}

		[IgnoreCast]
		private class ChuckNorrisFactApiResponse
		{
			public extern string Type { [Template("type")] get; }
			public extern FactDetails Value { [Template("value")] get; }

			[IgnoreCast]
			public class FactDetails
			{
				public extern int Id { [Template("id")] get; }
				public extern string Joke { [Template("joke")]get; }
			}
		}
	}
}
