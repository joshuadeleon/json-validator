using NJsonSchema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Transtatic.Json.Validation.Enums;
using Transtatic.Json.Validation.Interfaces;


namespace Transtatic.Json.Validation.Models {
	/// <summary>
	/// Results of a data validation of a json endpoint with string valued data
	/// </summary>
	public class DataValidationReport
	{
		#region Properties
		public Uri Endpoint { get; set; }
		public EntityReportCollection EntityReports { get; set; }
		public bool HasErrors { get { return EntityReports.TotalPropertyErrors > 0; } }
		public bool HasWarnings { get { return EntityReports.TotalPropertyWarnings > 0; } }
		public HttpMethod HttpMethod { get; set; }
		public TimeSpan ResponseTime { get; set; }
		public string RequestBody { get; set; }
		public string Schema { get; private set; }
		public double RecordErrorPercentage { get { return GetPercentage(EntityReports.RecordsWithErrors, TotalRecordsEvaluated); } }
		public double FieldErrorPercentage { get { return GetPercentage(EntityReports.TotalPropertyErrors, TotalFieldsEvaluated); } }
		public int TotalRecordsEvaluated { get; private set; }
		public int TotalFieldsEvaluated { get; private set; }
		public int ValidRecordCount { get { return TotalRecordsEvaluated - EntityReports.RecordsWithErrors; } }
		#endregion

		#region Constructors
		public DataValidationReport(JsonSchema4 schema, int totalEntities, int totalProperties)
		{
			EntityReports = new EntityReportCollection();
			Schema = schema.ToJson();
			TotalRecordsEvaluated = totalEntities;
			TotalFieldsEvaluated = totalProperties;
		}

		public DataValidationReport(HttpResponseMessage httpRequestMessage, JsonSchema4 schema, int totalEntities, int totalProperties)
			: this(schema, totalEntities, totalProperties)
		{
			Endpoint = httpRequestMessage.RequestMessage.RequestUri;
			HttpMethod = httpRequestMessage.RequestMessage.Method;
		}
		#endregion

		#region Methods
		private double GetPercentage(int count, int total)
		{
			return count / (double)total * 100;
		}

		public override string ToString()
		{
			var errorPlurality = (EntityReports.RecordsWithErrors == 1) ? $"was {EntityReports.RecordsWithErrors} entity error" : $"where {EntityReports.RecordsWithErrors} entity errors";
			return $"There {errorPlurality}.";
		}

		//	TODO: Implement
		public async Task<StreamWriter> WriteToSteam()
		{
			throw new NotImplementedException();
		}

		//	TODO: Refactor into stream? 
		public string WriteToString()
		{
			var lineBreak = "\n----------------------------------------------------------------\n";
			var builder = new StringBuilder();

			builder.AppendLine($"Validation Results");
			builder.AppendLine($"Data Validation Endpoint: {Endpoint.AbsoluteUri})");
			builder.AppendLine($"Http method: {HttpMethod}");
			builder.AppendLine($"Endpoint Response Time: {ResponseTime.TotalSeconds} seconds");
			builder.AppendLine(lineBreak);
			builder.AppendLine("Error Statistics\n");
			builder.AppendLine("Data Record Errors");
			builder.AppendLine($"Total Number of records validated: {TotalRecordsEvaluated}");
			builder.AppendLine($"Number of records with errors: {EntityReports.RecordsWithErrors}");
			builder.AppendLine($"Percentage of records with errors: {RecordErrorPercentage}%");
			builder.AppendLine();
			builder.AppendLine("Property Error Statistics\n");
			builder.AppendLine($"Total Number of fields validated: {TotalFieldsEvaluated}");
			builder.AppendLine($"Total number of fields with errors: {EntityReports.TotalPropertyErrors}");
			builder.AppendLine($"Percentage of fields with errors: {FieldErrorPercentage}%");
			builder.AppendLine(lineBreak);
			builder.AppendLine("Warnings\n");
			builder.AppendLine($"Number of records with warnings: {EntityReports.RecordsWithWarnings}");
			builder.AppendLine($"Percentage of total records with warnings: {GetPercentage(EntityReports.RecordsWithWarnings, TotalRecordsEvaluated)}%");
			builder.AppendLine($"Total number of warnings: {EntityReports.TotalPropertyWarnings}");
			builder.AppendLine(lineBreak);

			//	Error Summary
			//	HACK: Quick solution to getting all error fields
			var requiredFields = EntityReports
									.SelectMany(x => x.Properties)
									.Where(x => x.Type == MessageType.Missing)
									.Select(x => x.Name)
									.Distinct();
			var errorFields = EntityReports
									.SelectMany(x => x.Properties)
									.Where(x => x.Type == MessageType.Error)
									.Select(x => x.Name)
									.Distinct();
			var warningFields = EntityReports
									.SelectMany(x => x.Properties)
									.Where(x => x.Type == MessageType.Warning)
									.Select(x => x.Name)
									.Distinct();

			//	HACK
			if (requiredFields.Any() || errorFields.Any() || warningFields.Any())
			{
				builder.AppendLine("Error Summary");
			}

			if (requiredFields.Any())
			{
				builder.AppendLine("Missing Fields");
				builder.AppendLine();
				foreach (var field in requiredFields)
				{
					builder.AppendLine($"\t{field}");
				}
			}

			if (errorFields.Any())
			{
				builder.AppendLine();
				builder.AppendLine("Fields with Errors");
				builder.AppendLine();
				foreach (var field in errorFields)
				{
					builder.AppendLine($"\t{field}");
				}
			}

			if (warningFields.Any())
			{
				builder.AppendLine();
				builder.AppendLine("Fields with Warnings");
				builder.AppendLine();
				foreach (var field in warningFields)
				{
					builder.AppendLine($"\t{field}");
				}
			}
			builder.AppendLine(lineBreak);

			//	Specific Errors

			if (EntityReports.Any())
			{
				builder.AppendLine("Error Details");
				builder.AppendLine();

				foreach (var entity in EntityReports)
				{
					builder.AppendLine($"Entity: {entity.Entity}");

					//	HACK: Quick solution
					var missing = entity.Properties.Where(x => x.Type == MessageType.Missing);
					var error = entity.Properties.Where(x => x.Type == MessageType.Error);
					var warning = entity.Properties.Where(x => x.Type == MessageType.Warning);

					if (missing.Any())
					{
						builder.AppendLine();
						builder.AppendLine("The following properties are required but missing:");
						foreach (var property in missing)
						{

							builder.AppendLine($"\t{property.Name}");
						}
					}

					if (error.Any())
					{
						builder.AppendLine();
						builder.AppendLine("The following properties have invalid data types:");
						foreach (var property in error)
							builder.AppendLine($"\tProperty: {property.Name}\tExpected Type: {((PropertyError)property).ExpectedType}\tValue: {property.Value}");
					}

					if (warning.Any())
					{
						builder.AppendLine();
						builder.AppendLine("\nThe following field/values were not in the schema.");
						foreach (var property in warning)
							builder.AppendLine($"\t{property.Name}");
					}

					builder.AppendLine(lineBreak);
				}


			}

			builder.AppendLine($"Data Schema: \n{Schema}\n");
			return builder.ToString();
		}
		#endregion
	}
}
