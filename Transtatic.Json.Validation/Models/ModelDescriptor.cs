using NJsonSchema;

namespace Transtatic.Json.Validation.Models {
	public class ModelDescriptor {
		#region Properties
		public string DataType { get; set; }
		public string FieldName { get; set; }
		public bool IsRequired { get; set; }

		public bool IsNullable { get; set; }
		public string Pattern { get; set; }
		#endregion

		#region Constructors
		public ModelDescriptor () { }
		public ModelDescriptor (JsonProperty property, bool isRequired) {
			if ((property.Type & (JsonObjectType.String | JsonObjectType.Null)) != 0) {
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
	}
}
