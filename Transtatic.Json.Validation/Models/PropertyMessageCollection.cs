using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transtatic.Json.Validation.Interfaces;

namespace Transtatic.Json.Validation.Models
{
	/// <summary>
	/// Represents a collection of Property Messages
	/// </summary>
	public class PropertyMessageCollection : IList<IPropertyMessage>
	{
		#region Properties
		private List<IPropertyMessage> items = new List<IPropertyMessage>();

		public int ErrorCount { get; private set; }
		public int MissingFieldCount { get; private set; }
		public int WarningCount { get; private set; }
		#endregion

		#region Methods
		private void IncrementCounts(IPropertyMessage item)
		{
			if (item.Type == Enums.MessageType.Error)
				++ErrorCount;
			else if (item.Type == Enums.MessageType.Missing)
				++MissingFieldCount;
			else if (item.Type == Enums.MessageType.Warning)
				++WarningCount;
		}

		private void DecrementCounts(IPropertyMessage item)
		{
			if (item.Type == Enums.MessageType.Error)
				--ErrorCount;
			else if (item.Type == Enums.MessageType.Missing)
				--MissingFieldCount;
			else if (item.Type == Enums.MessageType.Warning)
				--WarningCount;
		}

		public void AddRange(IEnumerable<IPropertyMessage> properties)
		{
			items.AddRange(properties);
			foreach (var property in properties)
			{
				IncrementCounts(property);
			}
		}
		#endregion

		#region IList Implementation
		public IPropertyMessage this[int index]
		{
			get { return items[index]; }
			set { items[index] = value; }
		}
		public void Add(IPropertyMessage item)
		{
			items.Add(item);
			IncrementCounts(item);
		}
		public void Clear() { items.Clear(); }
		public bool Contains(IPropertyMessage item) { return items.Contains(item); }
		public int Count { get { return items.Count; } }
		public void CopyTo(IPropertyMessage[] array, int arrayIndex) { items.CopyTo(array, arrayIndex); }
		public bool IsReadOnly { get { return false; } }
		public IEnumerator<IPropertyMessage> GetEnumerator() { return items.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public int IndexOf(IPropertyMessage item) { return items.IndexOf(item); }
		public void Insert(int index, IPropertyMessage item)
		{
			items.Insert(index, item);
			IncrementCounts(item);
		}
		public bool Remove(IPropertyMessage item)
		{
			var isSuccess = items.Remove(item);
			if (isSuccess) { DecrementCounts(item); }
			return isSuccess;
		}
		public void RemoveAt(int index)
		{
			items.RemoveAt(index);
			DecrementCounts(items[index]);
		}
		#endregion
	}
}
