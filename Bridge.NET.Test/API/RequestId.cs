using System;

namespace Bridge.NET.Test.API
{
	/// <summary>
	/// Now that all interactions are handled via the dispatcher, there will be times where it is important to tie a result action to a request action - for
	/// example, if there are "New Email" forms on a page and "Send" is clicked on one of them, that form should be closed when the Send-Succeeded action is
	/// received. But only the form that tried to send the email should be closed, the other form should remain open. The SendEmail function should return a
	/// RequestId, process the work asychronously and then post an completed action that has a reference to that same RequestId. In other cases, it's useful
	/// to know which RequestId of two was most recently generated - if a "Data Received" action is retrieved by a Store then it might choose to accept that
	/// data if the associated RequestId is either the RequestId associated with the original request for data OR if the RequestId is newer.
	/// </summary>
	public class RequestId
	{
		private static DateTime _timeOfLastId = DateTime.MinValue;
		private static int _offsetOfLastId = 0;

		private readonly DateTime _requestTime;
		private readonly int _requestOffset;
		public RequestId()
		{
			_requestTime = DateTime.Now;
			if (_timeOfLastId < _requestTime)
			{
				_offsetOfLastId = 0;
				_timeOfLastId = _requestTime;
			}
			else
				_offsetOfLastId++;
			_requestOffset = _offsetOfLastId;
		}

		public bool ComesAfter(RequestId other)
		{
			if (other == null)
				throw new ArgumentNullException("other");

			if (_requestTime == other._requestTime)
				return _requestOffset > other._requestOffset;
			return (_requestTime > other._requestTime);
		}
	}
}