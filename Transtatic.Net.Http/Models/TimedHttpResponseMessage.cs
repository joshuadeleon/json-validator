using System;
using System.Net.Http;

namespace Transtatic.Net.Http.Models {
	public class TimedHttpResponseMessage {
		#region Properties
		public HttpResponseMessage Response { get; set; }
		public TimeSpan ResponseTime { get; set; }
		#endregion

		#region Constructors
		public TimedHttpResponseMessage(HttpResponseMessage response, TimeSpan responseTime) {
			Response = response;
			ResponseTime = responseTime;
		}
		#endregion
	}
}
