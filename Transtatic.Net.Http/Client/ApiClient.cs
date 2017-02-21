using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace Transtatic.Net.Http.Client {
	using Models;
	public class ApiClient {
		/// <summary>
		/// Returns an HttpClient object with an accept header set to JSON
		/// </summary>
		/// <returns></returns>
		public static HttpClient JsonHttpClient() {
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return httpClient;
		}

		/// <summary>
		/// Get data from the given url
		/// </summary>
		/// <param name="url">The url to GET data from</param>
		/// <returns>The http response</returns>
		public static async Task<HttpResponseMessage> GetAsync(string url) {
			using (var httpClient = JsonHttpClient())
			using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)) {
				return await httpClient.SendAsync(httpRequestMessage);
				//return await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
			}
		}

		/// <summary>
		/// Gets a timed response from the given url
		/// </summary>
		/// <param name="url">The url to GET data from</param>
		/// <returns>The timed response of the data from the given url</returns>
		public static async Task<TimedHttpResponseMessage> GetTimedAsync(string url) {
			using (var httpClient = JsonHttpClient())
			using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)) {
				var stopWatch = new Stopwatch();

				stopWatch.Start();   //	TODO: probably not high resolution but accurate enough for current purpose
				var response = await httpClient.SendAsync(httpRequestMessage);
				stopWatch.Stop();

				return new TimedHttpResponseMessage(response, stopWatch.Elapsed);
				//return await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
			}
		}

		/// <summary>
		/// Sends data async with an optional bearer token
		/// </summary>
		/// <param name="httpMethod">The http verb</param>
		/// <param name="uri">The uri for the request</param>
		/// <param name="bearerToken">The bearer token for the request</param>
		/// <param name="data">The data to send in the request</param>
		/// <returns>The result of the http request</returns>
		public static async Task<TimedHttpResponseMessage> SendAsync(HttpMethod httpMethod, Uri uri, string bearerToken, string data) {
			var stopWatch = new Stopwatch();

			using (var httpClient = JsonHttpClient())
			using (var httpContent = new StringContent(data, Encoding.UTF8))
			using (var httpRequestMessage = new HttpRequestMessage(httpMethod, uri)) {
				if (!string.IsNullOrEmpty(bearerToken))
					httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

				if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
					httpRequestMessage.Content = httpContent;

				stopWatch.Start();
				var response = await httpClient.SendAsync(httpRequestMessage);
				stopWatch.Stop();

				response.EnsureSuccessStatusCode();

				return new TimedHttpResponseMessage(response, stopWatch.Elapsed);
			}
		}
	}
}

