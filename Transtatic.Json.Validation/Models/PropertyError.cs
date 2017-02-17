namespace Transtatic.Json.Validation.Models {
	public class PropertyError {
		#region Properties
		public string Value { get; set; }
		public string ErrorMessage {
			get {
				return $"The {PropertyName} property was expected to be of type {ExpectedType} but was given {Value}";
			}
		}
		public string ExpectedType { get; set; }
		public string PropertyName { get; set; }
		#endregion

		#region Constructors
		public PropertyError(string propertyName, string expectedType, string value) {
			Value = value;
			ExpectedType = expectedType;
			PropertyName = propertyName;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns the Error message for this property.
		/// </summary>
		/// <returns>An error message for this property</returns>
		public override string ToString() {
			return ErrorMessage;
		}
		#endregion
	}
}
