namespace Transtatic.Json.Validation.Models {
	/// <summary>
	/// Property error report for a record (entity) in a data validation
	/// </summary>
	public class PropertyError : Property {
		#region Properties
		public string ErrorMessage {
			get {
				return $"The {Name} property was expected to be of type {ExpectedType} but was given {Value}";
			}
		}
		public string ExpectedType { get; set; }
		#endregion

		#region Constructors
		public PropertyError(string name, string expectedType, string value) : base(name, value) {
			ExpectedType = expectedType;
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
