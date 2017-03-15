using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transtatic.Json.Validation.Models
{
	/// <summary>
	/// Represents a missing property in the data which is required by the schema
	/// </summary>
	public class RequiredPropertyError : PropertyMessage
	{
		#region Properties
		public const string messageFormat = "The {0} property is required and is missing.";
		#endregion

		#region Constructors
		public RequiredPropertyError(string name) : base(name, "", string.Format(messageFormat, name), Enums.MessageType.Missing) { }
		#endregion

		#region Methods
		/// <summary>
		/// Returns the Error message for this property.
		/// </summary>
		/// <returns>An error message for this property</returns>
		public override string ToString()
		{
			return Message;
		}
		#endregion
	}
}
