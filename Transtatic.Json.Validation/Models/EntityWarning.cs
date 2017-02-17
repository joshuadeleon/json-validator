using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Transtatic.Json.Validation.Models {
	public class EntityWarning {
		#region Properties
		public string Entity { get; private set; }
		public bool HasWarnings { get { return UnknownProperties.Any(); } }
		public ICollection<UnknownProperty> UnknownProperties { get; private set; }
		#endregion

		#region Constructors
		public EntityWarning(object entity) {
			Entity = JsonConvert.SerializeObject(entity);
			UnknownProperties = new List<UnknownProperty>();
		}
		#endregion

		#region Methods
		public override string ToString() {
			return $"{Entity} has {UnknownProperties.Count} unknown property warnings.";
		}
		#endregion
	}
}
