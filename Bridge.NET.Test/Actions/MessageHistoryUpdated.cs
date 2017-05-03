﻿using Bridge.NET.Test.API;
using Bridge.React;
using ProductiveRage.Immutable;
using RequestId = ProductiveRage.Immutable.RequestId;

namespace Bridge.NET.Test.Actions
{
	public class MessageHistoryUpdated : IDispatcherAction, IAmImmutable
	{

		public MessageHistoryUpdated(RequestId requestId, Set<SavedMessageDetails> messages)
		{
			this.CtorSet(_ => _.RequestId, requestId);
			this.CtorSet(_ => _.Messages, messages);
		}
		public RequestId RequestId { get; private set; }
		public Set<SavedMessageDetails> Messages { get; private set; }
	}
}
