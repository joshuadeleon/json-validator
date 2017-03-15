using Transtatic.Json.Validation.Interfaces;

namespace Transtatic.Json.Validation.Models {
	/// <summary>
	/// Represents a property found in the data but not in the schema
	/// </summary>
	public class UnknownProperty : PropertyMessage
	{
		#region Properties
		private const string messageFormat = "Property: {0} with Value: {1} is not in the schema.";
		#endregion

		#region Constructors
		public UnknownProperty(string name, string value) : base(name, value, string.Format(messageFormat, name, value), Enums.MessageType.Warning) { }
		#endregion

		#region Methods
		public override string ToString()
		{
			return Message;
		}
		#endregion
	}
}
