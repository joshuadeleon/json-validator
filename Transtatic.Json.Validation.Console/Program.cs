using Newtonsoft.Json;
using NJsonSchema;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Transtatic.Json.Validation;
using Transtatic.Json.Validation.Utilities;
using Transtatic.Net.Http.Client;

namespace Transtatic.Json.Validate.Console {
	class Program {
		private static Logger logger = LogManager.GetLogger("Main logger");

		static void Main(string[] args) {
			var urlPath = @"http://someurl";			
			var data = "";
			var schema = Schema.Get<Foo>();

			var app = new RunApp();
			Task.Run(async () => await app.Run(urlPath, schema, data)).Wait();

			System.Console.WriteLine("Results written to file");
			//System.Console.WriteLine("Press any key to close");
			//System.Console.ReadKey();
		}

		public class RunApp {
			public async Task Run(string url, JsonSchema4 schema, string data) {

				try {
					var result = await ApiClient.SendAsync(HttpMethod.Post, new Uri(url), null, data);
					var json = await JsonTools.Get(result.Response);

					var validateResult = StringData.Validate(result, schema, json);
					var fileContents = validateResult.WriteToString();

					File.WriteAllText("output.md", fileContents);
				}
				catch (InvalidOperationException ioe) {
					logger.Debug(ioe, "");
				}
				catch (ArgumentException ae) {
					logger.Debug(ae, "");
				}
				catch (HttpRequestException hre) {
					logger.Debug(hre);
				}
				catch (Exception e) {
					logger.Debug(e.Message);
				}
			}
		}

		//	Insert Classes to test here
		public class Foo {
			[Required]
			public int Id { get; set; }
			
			[Required]
			[RegularExpression(@"^[A-Za-z\\s]+$")]
			public string Name { get; set; }
		}
	}
}
