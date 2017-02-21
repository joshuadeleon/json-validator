using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transtatic.Json.Validation.Interfaces;

namespace Transtatic.Json.Validation.Models {
	/// <summary>
	/// Represents a property (field) in the data schema
	/// </summary>
	public class Property : IProperty {
		#region Properties
		public string Name { get; set; }
		public string Value { get; set; }
		#endregion

		#region Constructors
		public Property(string name, string value) {
			Name = name;
			Value = value;
		}
		#endregion
	}
}
