using NJsonSchema;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Transtatic.Net.Http.Models;
using Xunit;

namespace Transtatic.Json.Validation.Tests.Json.Validation {

	public class DataValidatorTests
	{
		//	Valid and invalid generic Json
		private const string invalidObject = "{\"Id\", \"Name\": Thing}";
		private const string invalidObjects = "[{\"Id\": 4, Name: Sam}]";

		private const string validJson = "[{\"One\": \"one\", \"Two\": \"two\"}, {\"Field1\": 1, \"Field2\": 2}]";
		private const string validJsonSingleObject = "{\"One\": 1, \"Two\": \"two\", \"Truth\": true}";


		//private const string nullStringJson = "[{\"Id\": \"3\", \"Name\": null, \"Truth\": null}]";
		//private const string validStringValuedJsonObjects = "[{\"Name\":\"true\",\"Id\":1,\"Truth\":false,\"Uuid\":\"74953881-5db7-45cf-bdd5-2c538e37327c\",\"Longs\":0,\"Fraction\":\"1.2\",\"DateTime\":\"0001-01-01T00:00:00+00:00\",\"Timespan\":\"00:00:00\"},{\"Name\":\"Jane\",\"Id\":\"2\",\"Truth\":\"false\",\"Uuid\":\"00000000-0000-0000-0000-000000000000\",\"Longs\":\"0\",\"Fraction\":\"3.0\",\"DateTime\":\"2017-02-09T23:32:21.86106+00:00\",\"Timespan\":\"00:00:00\"},{\"Name\":\"Name\",\"Id\":\"3\",\"Truth\":\"true\",\"Uuid\":\"65ca6605-ef98-44ff-9f27-8c0b270b3786\",\"Longs\":\"0\",\"Fraction\":\"0.0\",\"DateTime\":\"0001-01-01T00:00:00+00:00\",\"Timespan\":\"00:00:00\"}]";

		JsonSchema4 testJsonSchema { get; set; }
		JsonSchema4 invalidJsonSchema { get; set; }

		DataValidator dataValidator = new DataValidator();
		public DataValidatorTests()
		{
			//	Get schema
			testJsonSchema = Utilities.Schema.Get<TestModels.TestSchema>();
		}

		[Theory]
		[InlineData(invalidObject)]
		[InlineData(invalidObjects)]
		public void FailIfInvalidJson(string json)
		{
			Assert.False(dataValidator.IsValidJson(json));
		}

		[Theory]
		[InlineData(validJson)]
		[InlineData(validJsonSingleObject)]
		public void PassesIfValidJson(string json)
		{
			Assert.True(dataValidator.IsValidJson(validJson));
		}

		[Fact]
		public void FailIfStringDataDoesNotMatchSchema()
		{
			//	Arrange
			var invalidSchema = @"[{""Field1"": ""one"", ""Field2"": ""two""}]";

			//	Act
			var results = dataValidator.Validate(testJsonSchema, invalidSchema);

			//	Assert
			Assert.True(results.HasErrors);
		}

		[Fact]
		public void PassesOnValidStringData()
		{
			//	Arrange
			var validSimpleJson = File.ReadAllText(@".\Json\validTestSchema.json");

			//	Act
			var results = dataValidator.Validate(testJsonSchema, validSimpleJson);

			//	Assert
			Assert.False(results.HasErrors);
		}

		[Fact]
		public void FailsIfStringDataNullWhenNotNullable()
		{
			//	Arrange
			var invalidNullValuedJson = "[{\"Id\": \"2\", \"Name\": null, \"Truth\": null}]";
			//	Act
			var results = dataValidator.Validate(testJsonSchema, invalidNullValuedJson);

			//	Assert
			Assert.True(results.HasErrors);
			Assert.Equal(1, results.EntityReports.Count);
		}

		public void HasEntityOnError()
		{
			//	Arrange
			var invalidStringValuedJsonObjects = "[{\"Name\":\"true\",\"Id\":1,\"Truth\": null,\"Uuid\":\"74953881-5db7-45cf-bdd5-2c538e37327c\",\"Longs\":0,\"Fraction\": \"true\",\"DateTime\": null,\"Timespan\":\"00:00:00\"},{\"Name\":\"Jane\",\"Id\":2,\"Truth\":false,\"Uuid\":\"00000000-0000-000000000000\",\"Longs\": \"s0\",\"Fraction\":3.0,\"DateTime\":\"2017-02-09T23:32:21.86106+00:00\",\"Timespan\":\"00:00:00\"},{\"Name\":\"Name\",\"Id\":3,\"Truth\":true,\"Uuid\":\"65ca6605-ef98-44ff-9f27-8c0b270b3786\",\"Longs\":0,\"Fraction\":0.0,\"DateTime\":\"0001-01-01\",\"Timespan\":\"1234\"}]";

			//	Act
			var results = dataValidator.Validate(testJsonSchema, invalidStringValuedJsonObjects);

			//	Assert
			Assert.NotNull(results.EntityReports.FirstOrDefault());
		}

		[Fact]
		public void CapturesUnknownProperties()
		{
			//	Arrange
			var unknownProperties = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Age"": ""32""}, {""Id"": ""1"", ""Name"": ""Asami"", ""Height"": ""70"", ""Foo"": ""this other thing""}]";

			//	Act
			var results = dataValidator.Validate(testJsonSchema, unknownProperties);

			//	Assert
			Assert.True(results.HasWarnings);
			Assert.Equal(2, results.EntityReports.Count);
			Assert.Equal(1, results.EntityReports.First().Properties.WarningCount);
			Assert.Equal(2, results.EntityReports.Skip(1).First().Properties.WarningCount);
		}

		[Fact]
		public void FailsOnInvalidEmail()
		{
			//	Arrange
			var invalidEmail = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Email"": ""not.an.email""}]";

			//	Act
			var results = dataValidator.Validate(testJsonSchema, invalidEmail);

			//	Assert
			Assert.True(results.HasErrors);
			Assert.Equal(1, results.EntityReports.Count);
		}

		[Theory]
		[InlineData(@"[{""Id"": ""1"", ""Name"": ""J_a_n_e""}]")]
		public void FailsOnInvalidRegex(string json)
		{
			//	Act
			var results = dataValidator.Validate(testJsonSchema, json);

			//	Assert
			Assert.True(results.HasErrors);
			Assert.Equal(1, results.EntityReports.Count);
		}

		[Fact]
		public void FailsOnInvalidPhone()
		{
			//	Arrange
			var invalidPhone = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Phone"": ""abc""}]";

			//	Act
			var results = dataValidator.Validate(testJsonSchema, invalidPhone);

			//	Assert
			Assert.True(results.HasErrors);
			Assert.Equal(1, results.EntityReports.Count);
			Assert.Equal("Phone", results.EntityReports.First().Properties.First().Name);
		}

		[Fact]
		public void HasValidEntityCount()
		{
			//	Arrange
			var validJson = File.ReadAllText(@".\Json\validTestSchema.json");

			//	Act
			var result = dataValidator.Validate(testJsonSchema, validJson);

			//	Arrange
			Assert.True(result.ValidRecordCount > 0);
			Assert.Equal(5, result.ValidRecordCount);
		}

		[Fact]
		public void HasTotalEntityCount()
		{
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidTestSchema.json");

			//	Act
			var result = dataValidator.Validate(testJsonSchema, invalidJson);

			//	Assert
			Assert.True(result.ValidRecordCount > 0);
			Assert.True(result.EntityReports.Count > 0);
			Assert.True(result.TotalRecordsEvaluated > 0);
		}

		[Fact]
		public void TotalRecordsEqualsSumValidAndInvalid()
		{
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidTestSchema.json");

			//	Act
			var result = dataValidator.Validate(testJsonSchema, invalidJson);

			//	Assert
			Assert.True(result.TotalRecordsEvaluated == result.ValidRecordCount + result.EntityReports.RecordsWithErrors);
		}


		[Fact]
		public void HasEntityErrorPercentage()
		{
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidTestSchema.json");

			//	Act
			var result = dataValidator.Validate(testJsonSchema, invalidJson);

			//	Arrange
			Assert.True(result.ValidRecordCount > 0);
			Assert.True(result.EntityReports.RecordsWithErrors > 0);
			Assert.True(result.RecordErrorPercentage > 0);
			Assert.Equal(20, result.RecordErrorPercentage);
		}

		[Fact]
		public void HasTotalPropertyCount()
		{
			//	Arrange
			var json = @"[{""Id"": ""23"", ""Name"": ""Jane""}, {""Id"": ""32"", ""Name"": ""Johnny""}]";

			//	Act
			var result = dataValidator.Validate(testJsonSchema, json);

			//	Assert
			Assert.True(result.TotalFieldsEvaluated > 0);
			Assert.Equal(4, result.TotalFieldsEvaluated);
		}

		[Fact]
		public void HasTotalPropertyErrorCount()
		{
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidTestSchema.json");

			//	Act
			var result = dataValidator.Validate(testJsonSchema, invalidJson);

			//	Arrange
			Assert.True(result.EntityReports.TotalPropertyErrors > 0);
			Assert.Equal(4, result.EntityReports.TotalPropertyErrors);
		}

		[Fact]
		public void HasTotalEntityPropertyErrorPercentage()
		{
			//	Arrange
			var invalidJson = File.ReadAllText(@".\Json\invalidTestSchema.json");

			//	Act
			var result = dataValidator.Validate(testJsonSchema, invalidJson);

			//	Arrange
			Assert.True(result.FieldErrorPercentage > 0);
		}

		[Fact]
		public void HasEndpointTiming()
		{
			//	Arrange
			var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://test.com") };
			var timedResponse = new TimedHttpResponseMessage(httpResponse, new TimeSpan(0, 0, 3));
			var json = @"[{""Id"": ""1"", ""Name"": ""Jane""}]";

			//	Act
			var result = dataValidator.Validate(timedResponse, testJsonSchema, json);

			//	Assert
			Assert.True(result.ResponseTime.TotalMilliseconds > 0);
			Assert.Equal(3, result.ResponseTime.Seconds);
		}

		[Fact]
		public void PassesNullableTypeWithRealNull()
		{
			//	Arrange
			var nullValuedJson = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Attribute"": null}]";

			//	Act
			var result = dataValidator.Validate(testJsonSchema, nullValuedJson);

			//	Assert
			Assert.False(result.HasErrors);
		}

		[Fact]
		public void CanHandleNullUnknownPropertyValues()
		{
			//	Arrange
			var nullUnknownProperty = @"[{""Id"": ""1"", ""Name"": ""Jane"", ""Stuff"": null}]";

			//	Act
			var result = dataValidator.Validate(testJsonSchema, nullUnknownProperty);

			//	Assert
			Assert.True(result.HasWarnings);
			Assert.Equal(1, result.EntityReports.SelectMany(x => x.Properties).Where(x => x.Type == Enums.MessageType.Warning).Count());
		}

	}
}
