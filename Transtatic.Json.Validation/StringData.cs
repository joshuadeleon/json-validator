using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using Transtatic.Json.Validation.Models;
using Transtatic.Net.Http.Models;


namespace Transtatic.Json.Validation {
	public static class StringData {
		
		#region Public Methods
		/// <summary>
		/// Validates JSON data types against given schema where JSON data values contain string representation data type
		/// </summary>
		/// <example>
		/// [{"Id": "1", "Name": "Jane", "IsPerson": "true"}]
		/// </example>
		/// <param name="schema">The JSON schema to validate data against.</param>
		/// <param name="json">The JSON data to validate.</param>
		/// <returns>ValidationResult containing schema errors.</returns>
		public static DataValidationResult Validate(HttpResponseMessage httpRequest, JsonSchema4 schema, string json) {
			var modelProperties = schema.Properties;
			var requiredProperties = new HashSet<string>(schema.RequiredProperties);

			//	TODO: this is simple and does not account for complex fields (arrays, objects)
			var modelDescriptors = modelProperties
											.Select(property => new ModelDescriptor(property.Value, requiredProperties.Contains(property.Key)))
											.ToDictionary(x => x.FieldName);

			var deserializedJson = JsonConvert.DeserializeObject<IEnumerable<ExpandoObject>>(json);

			return ValidateStringValues(httpRequest, schema, deserializedJson, requiredProperties, modelDescriptors);
		}

		//	If json not http based
		public static DataValidationResult Validate(JsonSchema4 schema, string json) {
			return Validate(new HttpResponseMessage() { RequestMessage = new HttpRequestMessage(HttpMethod.Get, default(Uri)) }, schema, json);
		}

		// Uses timing
		public static DataValidationResult Validate(TimedHttpResponseMessage timedResponse, JsonSchema4 schema, string json) {
			var validationResult = Validate(timedResponse.Response, schema, json);
			validationResult.ResponseTime = timedResponse.ResponseTime;

			return validationResult;
		}

		//	TODO: Allow for streaming data
		public static DataValidationResult Validate(JsonSchema4 schema, JsonTextReader jsonReader) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines if JSON is valid
		/// Credit: http://stackoverflow.com/questions/14977848/how-to-make-sure-that-string-is-valid-json-using-json-net
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool IsValidJson(string input) {
			input = input.Trim();
			if ((input.StartsWith("{") && input.EndsWith("}")) || //For object
				 (input.StartsWith("[") && input.EndsWith("]"))) //For array
			{
				try {
					var obj = JToken.Parse(input);
					return true;
				}
				catch (JsonReaderException) {
					return false;
				}
				catch (Exception)		
				{
					return false;
				}
			}
			else {
				return false;
			}
		}

		#endregion

		#region Private Methods

		private static DataValidationResult ValidateStringValues(HttpResponseMessage httpRequest, JsonSchema4 schema, IEnumerable<ExpandoObject> jsonData, HashSet<string> requiredProperties, IDictionary<string, ModelDescriptor> modelDescriptors) {
			var validationResults = new DataValidationResult(httpRequest, schema);

			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			foreach (var dataItem in jsonData) {
				var entityErrors = new EntityError(dataItem);
				var entityWarnings = new EntityWarning(dataItem);

				//	Sets Errors for missing properties in json data
				var missingProperties = HasRequiredProperties(requiredProperties, dataItem);
				if (missingProperties.Any()) {
					entityErrors.MissingPropertyErrors = missingProperties;
				}

				//	Sets Errors for invalid data types in values
				foreach (var property in dataItem) {
					validationResults.TotalPropertyCount++;
					if (modelDescriptors.ContainsKey(property.Key)) {
						var modelDescriptor = modelDescriptors[property.Key];
						var isTypeMatch = false;
						try {
							if (!modelDescriptor.IsNullable || property.Value != null) {
								isTypeMatch = TryDataConversion(modelDescriptor, property.Value);
							}

							//	Valid if property is nullable and value is null
							if (modelDescriptor.IsNullable && property.Value == null)
								continue;

							//	Add to Errors if conversion fails
							if (!isTypeMatch) {
								entityErrors.PropertyErrors.Add(new PropertyError(property.Key, modelDescriptor.DataType, property.Value.ToString()));
							}
						}
						catch (InvalidCastException) {
							entityErrors.PropertyErrors.Add(new PropertyError(property.Key, modelDescriptor.DataType, property.Value.ToString()));
						}
						catch (NullReferenceException) {
							entityErrors.PropertyErrors.Add(new PropertyError(property.Key, modelDescriptor.DataType, "NULL"));
						}
					}
					else {
						entityWarnings.UnknownProperties.Add(new UnknownProperty(property.Key, property.Value.ToString()));
					}

					
					//	TODO: any action if property isn't in model?
				}

				//	Add to validation results if there are errors
				if (entityErrors.HasErrors) {
					validationResults.EntityErrors.Add(entityErrors);
				}

				if (entityWarnings.HasWarnings)
					validationResults.EntityWarnings.Add(entityWarnings);

				if (!entityErrors.HasErrors) {
					validationResults.ValidEntityCount++;
				}

				validationResults.TotalEntityCount++;
			}
			sw.Stop();
			return validationResults;
		}

		/// <summary>
		/// Determines if the data given contains the required properties given by the schema
		/// </summary>
		/// <param name="requiredProperties"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		private static IEnumerable<MissingPropertyError> HasRequiredProperties(HashSet<string> requiredProperties, ExpandoObject data) {
			var missingProperties = requiredProperties.Except(((IDictionary<string, object>)data).Keys);

			if (missingProperties.Any())
				return missingProperties.Select(key => new MissingPropertyError(key));

			return Enumerable.Empty<MissingPropertyError>();
		}

		/// <summary>
		/// Tries to convert the given value to the given format
		/// </summary>
		/// <param name="dataType">The data type of the given value</param>
		/// <param name="value">The value to type check</param>
		/// <returns>True of successful, otherwise throws exception</returns>
		private static bool TryDataConversion(ModelDescriptor descriptor, object data) {
			//	Gaurd against null reference
			if (data == null)
				throw new NullReferenceException();

			var value = data.ToString();

			switch (descriptor.DataType) {
				case "bool":
					Convert<bool>(value);
					break;
				case "date-time":
					Convert<DateTimeOffset>(value);
					break;
				case "double":
					Convert<double>(value);
					break;
				case "email":
					Convert<string>(value);
					var emailValidator = new EmailAddressAttribute();
					return emailValidator.IsValid(value);
				case "int32":
					Convert<int>(value);
					break;
				case "int64":
					Convert<long>(value);
					break;
				case "guid":
					Convert<Guid>(value);
					break;
				case "phone":
					Convert<string>(value);
					var phoneValidator = new PhoneAttribute();
					return phoneValidator.IsValid(value);
				case "string":
					Convert<string>(value);

					if (!string.IsNullOrEmpty(descriptor.Pattern)) {
						var regexValidator = new RegularExpressionAttribute(descriptor.Pattern);
						return regexValidator.IsValid(value);
					}
					break;
				case "time-span":
					Convert<TimeSpan>(value);
					break;
				default:
					//	Log error
					throw new NotImplementedException(string.Format("The format: {0} with value: {1} has no corresponding cast", descriptor.DataType, value));
			}

			return true;
		}

		/// <summary>
		/// Converts string to the give type T
		/// Credit: http://stackoverflow.com/questions/2961656/generic-tryparse
		/// </summary>
		/// <typeparam name="T">Type to converto</typeparam>
		/// <param name="input">Value to convert</param>
		/// <returns></returns>
		private static T Convert<T>(string input) {
			try {
				var converter = TypeDescriptor.GetConverter(typeof(T));

				if (converter != null) {	
					return (T)converter.ConvertFromString(input);
				}
				return default(T);
			}
			catch (NotSupportedException) {
				return default(T);
			}
			catch (Exception e) {
				throw new InvalidCastException(string.Format("Value: {0} does not match schema type {1}", input, typeof(T).ToString()), e);
			}
		}
	}
	#endregion
}

