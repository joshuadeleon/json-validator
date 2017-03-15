using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Transtatic.Json.Validation.Tests.TestModels
{
	public class TestSchema
	{
		[Required]
		[RegularExpression("^[A-Za-z0-9\\s]+$")]
		public string Name { get; set; }
		[Required]
		public int Id { get; set; }
		public bool Truth { get; set; }
		public Guid Uuid { get; set; }
		public long Longs { get; set; }
		public double Fraction { get; set; }
		public DateTimeOffset DateTime { get; set; }
		public TimeSpan Timespan { get; set; }
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		[Phone]
		public string Phone { get; set; }
		public string Attribute { get; set; }
	}
}
