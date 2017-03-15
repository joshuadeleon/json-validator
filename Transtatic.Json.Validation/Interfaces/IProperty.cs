using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transtatic.Json.Validation.Interfaces {
	/// <summary>
	/// Abstracts a field/property a its value
	/// </summary>
	public interface IProperty
	{
		string Name { get; set; }
		string Value { get; set; }
	}
}
