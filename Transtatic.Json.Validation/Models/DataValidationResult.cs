using NJsonSchema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Transtatic.Json.Validation.Models {
	/// <summary>
	/// The results of a schema validation
	/// </summary>
	public class DataValidationResult {
		#region Properties
		public Uri Endpoint { get; set; }
		public ICollection<EntityError> EntityErrors { get; private set; }
		public double EntityErrorPercentage { get { return EntityErrors.Count / (double)TotalEntityCount * 100; } }
		public ICollection<EntityWarning> EntityWarnings { get; private set; }
		public bool HasErrors { get { return EntityErrors.Any(); } }
		public bool HasWarnings { get { return EntityWarnings.Any(); } }
		public HttpMethod HttpMethod { get; set; }
		public double PropertyErrorPercentage { get { return TotalPropertyErrorCount / (double)TotalPropertyCount * 100; } }
		public TimeSpan ResponseTime { get; set; }
		public string RequestBody { get; set; }
		public string Schema { get; private set; }

		public int TotalEntityCount { get; set; }
		public int TotalPropertyCount { get; set; }
		public int TotalPropertyErrorCount {
			get { //	HACK: Not the best implementation, refactor
				return EntityErrors
							.SelectMany(x => x.PropertyErrors)
							.Count();
			}
		}

		public int TotalPropertyWarningCount {
			get { //	HACK: Not the best implementation, refactor
				return EntityWarnings
							.SelectMany(x => x.UnknownProperties)
							.Count();
			}
		}
		public int ValidEntityCount { get; set; }
		#endregion

		#region Constructors
		public DataValidationResult(JsonSchema4 schema) {
			EntityErrors = new List<EntityError>();
			EntityWarnings = new List<EntityWarning>();
			Schema = schema.ToJson();
		}

		public DataValidationResult(HttpResponseMessage httpRequestMessage, JsonSchema4 schema) : this(schema) {
			Endpoint = httpRequestMessage.RequestMessage.RequestUri;
			HttpMethod = httpRequestMessage.RequestMessage.Method;
		}
		#endregion

		#region Methods
		public override string ToString() {
			var errorPlurality = (EntityErrors.Count == 1) ? $"was {EntityErrors.Count} entity error" : $"where {EntityErrors.Count} entity errors";
			return $"There {errorPlurality}.";
		}

		//	TODO: Implement
		public async Task<StreamWriter> WriteToSteam() {
			throw new NotImplementedException();
		}

		//	TODO: Refactor into stream? 
		public string WriteToString() {
			var lineBreak = "\n----------------------------------------------------------------\n";
			var builder = new StringBuilder();


			builder.AppendLine($"# Validation Results");
			builder.AppendLine($"\n**Data Validation Endpoint:** [{Endpoint.AbsoluteUri}]({Endpoint.AbsoluteUri})");
			builder.AppendLine($"\n**Http method:** {HttpMethod}");
			builder.AppendLine($"\n**Endpoint Response Time:** {ResponseTime.TotalSeconds} seconds");
			builder.AppendLine(lineBreak);
			builder.AppendLine("## Error Statistics");
			builder.AppendLine("### Data Record Errors");
			builder.AppendLine($"\n**Total Number of records validated:** {TotalEntityCount}");
			builder.AppendLine($"\n**Number of records with errors:** {EntityErrors.Count}");
			builder.AppendLine($"\n**Percentage of records with errors:** {EntityErrorPercentage}%");
			builder.AppendLine();
			builder.AppendLine("### Property Error Statistics");
			builder.AppendLine($"\n**Total Number of properties validated:** {TotalPropertyCount}");
			builder.AppendLine($"\n**Total number of fields with errors:** {TotalPropertyErrorCount}");
			builder.AppendLine($"\n**Percentage of fields with errors:** {PropertyErrorPercentage}%");
			builder.AppendLine(lineBreak);
			builder.AppendLine("## Warnings");
			builder.AppendLine($"\n**Number of records with warnings:** {EntityWarnings.Count}");
			builder.AppendLine($"\n**Total number of warnings:** {TotalPropertyWarningCount}");
			builder.AppendLine();
			builder.AppendLine(lineBreak);

			if (HasErrors || HasWarnings) {
				foreach (var entity in EntityErrors) {
					builder.AppendLine($"Entity: {entity.Entity}");

					//	Write missing required properties
					if (entity.MissingPropertyErrors.Any()) {
						builder.AppendLine();
						builder.AppendLine("The following properties are required but missing:");
						foreach (var propertyError in entity.MissingPropertyErrors) {
							builder.AppendLine($"\n\t{propertyError.PropertyName}");
						}
					}

					//	Write Invalid data types
					if (entity.PropertyErrors.Any()) {
						builder.AppendLine();
						builder.AppendLine("The following properties have invalid data types:");
						foreach (var propertyError in entity.PropertyErrors)
							builder.AppendLine($"\n\tProperty: {propertyError.PropertyName}\tExpected Type: {propertyError.ExpectedType}\tValue: {propertyError.Value}");
					}

					builder.AppendLine(lineBreak);
				}

				foreach (var entity in EntityWarnings) {
					builder.AppendLine($"Entity: {entity.Entity}");

					if (entity.UnknownProperties.Any()) {
						builder.AppendLine();
						builder.AppendLine("\nThe following field/values were not in the schema.");
						foreach (var property in entity.UnknownProperties)
							builder.AppendLine($"\n\t{property.PropertyName}: {property.Value}");
					}
					builder.AppendLine(lineBreak);
				}

				builder.AppendLine();
				builder.AppendLine($"\nData Schema: \n\n```{Schema}```");
			}

			return builder.ToString();

		}
		#endregion

	}
}
