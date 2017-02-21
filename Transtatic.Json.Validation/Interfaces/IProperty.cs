using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transtatic.Json.Validation.Interfaces {
	public interface IProperty {
		string Name { get; set; }
		string Value { get; set; } 
	}
}
