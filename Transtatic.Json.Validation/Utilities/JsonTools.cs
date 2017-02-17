using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Transtatic.Json.Validation.Utilities {
	public class JsonTools {
		public static async Task<string> Get(HttpResponseMessage response) {
			using (var stream = await response.Content.ReadAsStreamAsync())
			using (var streamReader = new StreamReader(stream))
				return await streamReader.ReadToEndAsync();
		}

		public static async Task<T> Deserialize<T>(HttpResponseMessage response) {
			using (var stream = await response.Content.ReadAsStreamAsync())
			using (var streamReader = new StreamReader(stream))
			using (var reader = new JsonTextReader(streamReader)) {
				JsonSerializer serializer = new JsonSerializer();
				return serializer.Deserialize<T>(reader);
			}
		}
	}
}
