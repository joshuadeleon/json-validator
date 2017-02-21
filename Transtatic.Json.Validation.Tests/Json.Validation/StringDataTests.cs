using NJsonSchema;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Transtatic.Net.Http.Models;
using Xunit;

namespace Transtatic.Json.Validation.Tests.Json.Validation {

	public class StringDataTests {
		//	Valid and invalid generic Json
		private const string invalidObject = "{\"Id\", \"Name\": Thing}";
		private const string invalidObjects = "[{\"Id\": 4, Name: Sam}]";

		private const string validJson = "[{\"One\": \"one\", \"Two\": \"two\"}, {\"Field1\": 1, \"Field2\": 2}]";
		private const string validJsonSingleObject = "{\"One\": 1, \"Two\": \"two\", \"Truth\": true}";


		//private const string nullStringJson = "[{\"Id\": \"3\", \"Name\": null, \"Truth\": null}]";
		//private const string validStringValuedJsonObjects = "[{\"Name\":\"true\",\"Id\":1,\"Truth\":false,\"Uuid\":\"74953881-5db7-45cf-bdd5-2c538e37327c\",\"Longs\":0,\"Fraction\":\"1.2\",\"DateTime\":\"0001-01-01T00:00:00+00:00\",\"Timespan\":\"00:00:00\"},{\"Name\":\"Jane\",\"Id\":\"2\",\"Truth\":\"false\",\"Uuid\":\"00000000-0000-0000-0000-000000000000\",\"Longs\":\"0\",\"Fraction\":\"3.0\",\"DateTime\":\"2017-02-09T23:32:21.86106+00:00\",\"Timespan\":\"00:00:00\"},{\"Name\":\"Name\",\"Id\":\"3\",\"Truth\":\"true\",\"Uuid\":\"65ca6605-ef98-44ff-9f27-8c0b270b3786\",\"Longs\":\"0\",\"Fraction\":\"0.0\",\"DateTime\":\"0001-01-01T00:00:00+00:00\",\"Timespan\":\"00:00:00\"}]";

		JsonSchema4 simpleJsonSchema { get; set; }
		JsonSchema4 invalidJsonSchema { get; set; }

		public StringDataTests() {
			//	Get schema
			simpleJsonSchema = Helpers.Schema.Get<Helpers.SchemaModels.SimpleJson>();
		}

		[Theory]
		[InlineData(invalidObject)]
		[InlineData(invalidObjects)]
		public void FailIfInvalidJson(string json) {
			Assert.False(StringData.IsValidJson(json));
		}

		[Theory]
		[InlineData(validJson)]
		[InlineData(validJsonSingleObject)]
		public void PassesIfValidJson(string json) {
			Assert.True(StringData.IsValidJson(validJson));
		}

		[Fact]
		public void FailIfStringDataDoesNotMatchSchema() {
			//	Arrange
			var invalidSchema = @"[{""Field1"": ""one"", ""Field2"": ""two""}]";

			//	Act
			var results = StringData.Validate(simpleJsonSchema, invalidSchema);

			//	Assert
			Assert.True(results.HasErrors);
		}

		[Fact]
		public void PassesOnValidStringData() {
			//	Arrange
			var validSimpleJson = File.ReadAllText(@".\Json\validSimpleSchema.json");

			//	Act
			var results = StringData.Validate(simpleJsonSchema, validSimpleJson);

			//	Assert
			Assert.False(results.HasErrors);
		}

		[Fact]
		public void FailsIfStringDataNullWhenNotNullable() {
			//	Arrange
			var invalidNullValuedJson = "[{\"Id\": \"2\", \"Name\": null, \"Truth\": null}]";
			//	Act
			var results = StringData.Validate(simpleJsonSchema, invalidNullValuedJson);

			//	Assert
			Assert.True(results.HasErrors);
			Assert.Equal(1, results.EntityErrors.Count);
		}

		public void HasEntityOnError() {
			//	Arrange
			var invalidStringValuedJsonObjects = "[{\"Name\":\"true\",\"Id\":1,\"Truth\": null,\"Uuid\":\"74953881-5db7-45cf-bdd5-2c538e37327c\",\"Longs\":0,\"Fraction\": \"true\",\"DateTime\": null,\"Timespan\":\"00:00:00\"},{\"Name\":\"Jane\",\"Id\":2,\"Truth\":false,\"Uuid\":\"00000000-0000-000000000000\",\"Longs\": \"s0\",\"Fraction\":3.0,\"DateTime\":\"2017-02-09T23:32:21.86106+00:00\",\"Timespan\":\"00:00:00\"},{\"Name\":\"Name\",\"Id\":3,\"Truth\":true,\"Uuid\":\"65ca6605-ef98-44ff-9f27-8c0b270b3786\",\"Longs\":0,\"Fraction\":0.0,\"DateTime\":\"0001-01-01\",\"Timespan\":\"1234\"}]";

			//	Act
			var results = StringData.Validate(simpleJsonSchema, invalidStringValuedJsonObjects);

			//	Assert
			Assert.NotNull(results.EntityErrors.FirstOrDefault());
		}

		[Fact]
		public void CapturesUnknownProperties() {
			//	Arrange
			var unknownProperties = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Age"": ""32""}, {""Id"": ""1"", ""Name"": ""Asami"", ""Height"": ""70"", ""Foo"": ""this other thing""}]";

			//	Act
			var results = StringData.Validate(simpleJsonSchema, unknownProperties);

			//	Assert
			Assert.True(results.HasWarnings);
			Assert.Equal(2, results.EntityWarnings.Count);
			Assert.Equal(1, results.EntityWarnings.First().UnknownProperties.Count);
			Assert.Equal(2, results.EntityWarnings.Skip(1).First().UnknownProperties.Count);
		}

		[Fact]
		public void FailsOnInvalidEmail() {
			//	Arrange
			var invalidEmail = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Email"": ""not.an.email""}]";

			//	Act
			var results = StringData.Validate(simpleJsonSchema, invalidEmail);

			//	Assert
			Assert.True(results.HasErrors);
			Assert.Equal(1, results.EntityErrors.Count);
		}

		[Theory]
		[InlineData(@"[{""Id"": ""1"", ""Name"": ""J_a_n_e""}]")]
		public void FailsOnInvalidRegex(string json) {
			//	Act
			var results = StringData.Validate(simpleJsonSchema, json);

			//	Assert
			Assert.True(results.HasErrors);
			Assert.Equal(1, results.EntityErrors.Count);
		}

		[Fact]
		public void FailsOnInvalidPhone() {
			//	Arrange
			var invalidPhone = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Phone"": ""abc""}]";

			//	Act
			var results = StringData.Validate(simpleJsonSchema, invalidPhone);

			//	Assert
			Assert.True(results.HasErrors);
			Assert.Equal(1, results.EntityErrors.Count);
			Assert.Equal("Phone", results.EntityErrors.First().PropertyErrors.First().PropertyName);
		}

		[Fact]
		public void HasValidEntityCount() {
			//	Arrange
			var validJson = File.ReadAllText(@".\Json\validSimpleSchema.json");

			//	Act
			var result = StringData.Validate(simpleJsonSchema, validJson);

			//	Arrange
			Assert.True(result.ValidEntityCount > 0);
			Assert.Equal(5, result.ValidEntityCount);
		}

		[Fact]
		public void HasTotalEntityCount() {
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidSimpleSchema.json");

			//	Act
			var result = StringData.Validate(simpleJsonSchema, invalidJson);

			//	Assert
			Assert.True(result.ValidEntityCount > 0);
			Assert.True(result.EntityErrors.Count > 0);
			Assert.True(result.TotalEntityCount > 0);
			Assert.True(result.TotalEntityCount == result.ValidEntityCount + result.EntityErrors.Count);
		}
		

		[Fact]
		public void HasEntityErrorPercentage() {
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidSimpleSchema.json");

			//	Act
			var result = StringData.Validate(simpleJsonSchema, invalidJson);

			//	Arrange
			Assert.True(result.ValidEntityCount > 0);
			Assert.True(result.EntityErrors.Count > 0);
			Assert.True(result.EntityErrorPercentage > 0);
		}

		[Fact]
		public void HasTotalPropertyCount() {
			//	Arrange
			var json = @"[{""Id"": ""23"", ""Name"": ""Jane""}, {""Id"": ""32"", ""Name"": ""Johnny""}]";

			//	Act
			var result = StringData.Validate(simpleJsonSchema, json);
			
			//	Assert
			Assert.True(result.TotalPropertyCount > 0);
			Assert.Equal(4, result.TotalPropertyCount);
		}

		[Fact]
		public void HasTotalPropertyErrorCount() {
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidSimpleSchema.json");

			//	Act
			var result = StringData.Validate(simpleJsonSchema, invalidJson);

			//	Arrange
			Assert.True(result.TotalPropertyErrorCount > 0);
			Assert.Equal(4, result.TotalPropertyErrorCount);
		}

		[Fact]
		public void HasTotalEntityPropertyErrorPercentage() {
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidSimpleSchema.json");

			//	Act
			var result = StringData.Validate(simpleJsonSchema, invalidJson);

			//	Arrange
			Assert.True(result.PropertyErrorPercentage > 0);
		}

		[Fact]
		public void HasEndpointTiming() {
			//	Arrange
			var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK) {  RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com") };
			var timedResponse = new TimedHttpResponseMessage(httpResponse, new TimeSpan(0, 0, 3));
			var json = @"[{""Id"": ""1"", ""Name"": ""Jane""}]";

			//	Act
			var result = StringData.Validate(timedResponse, simpleJsonSchema, json);

			//	Assert
			Assert.True(result.ResponseTime.TotalMilliseconds > 0);
			Assert.Equal(3, result.ResponseTime.Seconds);
		}

		[Fact]
		public void PassesNullableTypeWithRealNull() {
			//	Arrange
			var nullValuedJson = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Attribute"": null}]";

			//	Act
			var result = StringData.Validate(simpleJsonSchema, nullValuedJson);

			//	Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void CanHandleNullUnknownPropertyValues() {
			//	Arrange
			var nullUnknownProperty = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Stuff"": null}]";

			//	Act
			var result = StringData.Validate(simpleJsonSchema, nullUnknownProperty);

			//	Assert
			Assert.True(result.HasWarnings);
			Assert.Equal(1, result.EntityWarnings.Count);
		}
	}
}
