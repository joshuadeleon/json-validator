using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Transtatic.Json.Validation.Models {
	public class EntityError {
		#region Properties
		public string Entity { get; private set; }
		public bool HasErrors { get { return PropertyErrors.Any() || MissingPropertyErrors.Any(); } }
		public IEnumerable<MissingPropertyError> MissingPropertyErrors { get; set; }
		public ICollection<PropertyError> PropertyErrors { get; private set; }		
		#endregion

		#region Constructors
		public EntityError(object entity) {
			Entity = JsonConvert.SerializeObject(entity);
			MissingPropertyErrors = Enumerable.Empty<MissingPropertyError>();
			PropertyErrors = new List<PropertyError>();
			
		}
		#endregion

		#region Methods
		public override string ToString() {
			var errorState = (HasErrors) ? "has" : "is free of";
			return $"{Entity} {errorState} errors.";
		}
		#endregion
	}
}
