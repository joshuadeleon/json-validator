using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transtatic.Json.Validation.Interfaces;
using Transtatic.Json.Validation.Enums;

namespace Transtatic.Json.Validation.Models
{
	/// <summary>
	/// Abstracts messaging about a data field/property
	/// </summary>
	public abstract class PropertyMessage : Property, IPropertyMessage
	{
		#region Properties
		public string Message { get; }
		public MessageType Type { get; private set; }
		#endregion

		#region Construtors
		public PropertyMessage(string name, string value, string message, MessageType type) : base(name, value)
		{
			Message = message;
			Type = type;
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			return $"{Name} has value {Value} is a {Type}.";
		}
		#endregion
	}
}
