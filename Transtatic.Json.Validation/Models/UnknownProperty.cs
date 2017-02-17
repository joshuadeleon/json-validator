namespace Transtatic.Json.Validation.Models {
	public class UnknownProperty {
		#region Properties
		public string PropertyName { get; set; }
		public string Value { get; set; }
		#endregion

		#region Constructors
		public UnknownProperty(string propertyName, string value) {
			PropertyName = propertyName;
			Value = value;
		}
		#endregion

		#region Methods
		public override string ToString() {
			return $"Propety: {PropertyName} with Value: {Value} is not in the schema.";
		}
		#endregion
	}
}
