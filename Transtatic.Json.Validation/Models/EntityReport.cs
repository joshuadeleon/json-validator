using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transtatic.Json.Validation.Interfaces;
using Newtonsoft.Json;

namespace Transtatic.Json.Validation.Models
{
	/// <summary>
	/// Error report for a record (entity) in a data validation
	/// </summary>
	public class EntityReport : IEntityValidation
	{
		#region Properties
		public string Entity { get; private set; }
		public bool HasItems { get { return Properties.Any(); } }
		public PropertyMessageCollection Properties { get; private set; }
		#endregion

		#region Constructors
		public EntityReport(object entity)
		{
			Properties = new PropertyMessageCollection();
			Entity = JsonConvert.SerializeObject(entity);
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			return $"This entity has {Properties.ErrorCount} error(s), {Properties.MissingFieldCount} missing field(s), and {Properties.WarningCount} properties not in the schema."; ;
		}
		#endregion
	}
}
