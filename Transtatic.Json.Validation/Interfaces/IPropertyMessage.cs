using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transtatic.Json.Validation.Enums;

namespace Transtatic.Json.Validation.Interfaces
{
	/// <summary>
	/// Abstracts a property with a message and type
	/// </summary>
	public interface IPropertyMessage : IProperty
	{
		string Message { get; }
		MessageType Type { get; }
	}
}
