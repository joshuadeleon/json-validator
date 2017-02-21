using Transtatic.Json.Validation.Interfaces;

namespace Transtatic.Json.Validation.Models {
	/// <summary>
	/// Represents a property which is found in the data but not in the schema
	/// </summary>
	public class UnknownProperty : IProperty {
		#region Properties
		public string Name { get; set; }
		public string Value { get; set; }
		#endregion

		#region Constructors
		public UnknownProperty(string name, string value) {
			Name = name;
			Value = value;
		}
		#endregion

		#region Methods
		public override string ToString() {
			return $"Property: {Name} with Value: {Value} is not in the schema.";
		}
		#endregion
	}
}
