using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transtatic.Json.Validation.Models;

namespace Transtatic.Json.Validation.Interfaces
{
	public interface IEntityValidation
	{
		string Entity { get; }
		bool HasItems { get; }
		PropertyMessageCollection Properties { get; }
	}
}
