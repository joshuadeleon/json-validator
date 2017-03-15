using NJsonSchema;

namespace Transtatic.Json.Validation.Models {
	/// <summary>
	/// Describes a data type
	/// </summary>
	public class DataDescriptor
	{
		#region Properties
		public string DataType { get; set; }
		public string FieldName { get; set; }
		public bool IsRequired { get; set; }
		public bool IsNullable { get; set; }
		public string Pattern { get; set; }
		#endregion

		#region Constructors
		public DataDescriptor() { }
		public DataDescriptor(JsonProperty property, bool isRequired)
		{
			if ((property.Type & (JsonObjectType.String | JsonObjectType.Null)) != 0)
			{
				DataType = (property.Format == null) ? "string" : property.Format;
				Pattern = property.Pattern;
			}
			else if (property.Type.HasFlag(JsonObjectType.Boolean))
				DataType = "bool";
			else
				DataType = property.Format;


			FieldName = property.Name;
			IsNullable = (property.Type & JsonObjectType.Null) == JsonObjectType.Null
								|| property.Type == JsonObjectType.String;
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			return $"FieldName: {FieldName} - DataType: {DataType} - Required: {IsRequired} - Nullable: {IsNullable}";
		}
		#endregion
	}
}
