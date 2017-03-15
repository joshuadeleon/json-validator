using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transtatic.Json.Validation.Models
{
	/// <summary>
	/// Represents a collection of Entity Reports
	/// </summary>
	public class EntityReportCollection : ICollection<EntityReport>
	{
		#region Properties
		//	Private
		private ICollection<EntityReport> entityReport = new List<EntityReport>();

		//	Public
		public int RecordsWithErrors { get; private set; }
		public int RecordsWithWarnings { get; private set; }
		public int TotalPropertyErrors { get; private set; }
		public int TotalPropertyWarnings { get; private set; }
		#endregion

		#region Methods
		private void IncrementCounts(EntityReport item)
		{
			if (item.Properties.WarningCount > 0)
				++RecordsWithWarnings;

			if (item.Properties.ErrorCount > 0 || item.Properties.MissingFieldCount > 0)
				++RecordsWithErrors;

			TotalPropertyWarnings += item.Properties.WarningCount;
			TotalPropertyErrors += item.Properties.MissingFieldCount + item.Properties.ErrorCount;
		}

		private void DecrementCounts(EntityReport item)
		{
			if (item.Properties.WarningCount > 0)
				--RecordsWithWarnings;

			if (item.Properties.ErrorCount > 0 || item.Properties.MissingFieldCount > 0)
				--RecordsWithErrors;

			TotalPropertyWarnings -= item.Properties.WarningCount;
			TotalPropertyErrors -= (item.Properties.MissingFieldCount + item.Properties.ErrorCount);
		}
		#endregion

		#region ICollection Implementation
		public void Add(EntityReport item)
		{
			entityReport.Add(item);
			IncrementCounts(item);
		}
		public void Clear() { entityReport.Clear(); }
		public int Count { get { return entityReport.Count; } }
		public bool Contains(EntityReport item) { return entityReport.Contains(item); }
		public void CopyTo(EntityReport[] array, int arrayIndex) { entityReport.CopyTo(array, arrayIndex); }
		public bool IsReadOnly { get { return false; } }
		public IEnumerator<EntityReport> GetEnumerator() { return entityReport.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public bool Remove(EntityReport item)
		{
			var isSuccess = entityReport.Remove(item);
			if (isSuccess) { DecrementCounts(item); }
			return isSuccess;
		}
		#endregion
	}
}
