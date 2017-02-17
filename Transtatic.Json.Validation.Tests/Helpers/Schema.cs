using NJsonSchema;
using System.Threading.Tasks;

namespace Transtatic.Json.Validation.Tests.Helpers {
	public static class Schema {
		public static JsonSchema4 Get<T>() where T : class {
			return Task.Run(() => JsonSchema4.FromTypeAsync<T>()).Result;
		}
	}
}
