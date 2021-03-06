﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transtatic.Json.Validation.Models
{
	/// <summary>
	/// Property error report for a record (entity) in a data validation
	/// </summary>
	public class PropertyError : PropertyMessage
	{
		#region Properties
		private const string messageFormat = "The {0} property was expected to be of type {1} but was given {2}";
		public string ExpectedType { get; set; }
		#endregion

		#region Constructors
		public PropertyError(string name, string value, string expectedType)
			: base(name, value, string.Format(messageFormat, name, expectedType, value), Enums.MessageType.Error)
		{
			ExpectedType = expectedType;
		}
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
