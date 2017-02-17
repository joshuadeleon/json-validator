namespace Transtatic.Json.Validation.Models {
	public class MissingPropertyError {
		public string ErrorMessage {
			get {
				return $"The {PropertyName} property is required and is missing.";
			}
		}
		public string PropertyName { get; set; }

		#region Constructors
		public MissingPropertyError(string propertyName) {
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
