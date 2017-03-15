using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Transtatic.Json.Validation.Models;
using Transtatic.Json.Validation.Enums;
using Transtatic.Json.Validation.Interfaces;

namespace Transtatic.Json.Validation.Tests.Json.Validation.Models
{
	public class PropertyMessageCollectionTests
	{
		UnknownProperty unknownProperty = new UnknownProperty("UnknownName", "UnknownValue");
		RequiredPropertyError requiredProperty = new RequiredPropertyError("RequiredProperty");
		PropertyError propertyError = new PropertyError("ErrorName", "ErrorValue", "ExpectedType");

		ICollection<IPropertyMessage> unknownProperties = new List<IPropertyMessage>();
		ICollection<IPropertyMessage> requiredProperties = new List<IPropertyMessage>();
		ICollection<IPropertyMessage> propertyErrors = new List<IPropertyMessage>();

		public PropertyMessageCollectionTests()
		{
			for (var i = 0; i < 10; ++i)
				unknownProperties.Add(new UnknownProperty($"UnknownName{i}", $"UnknownValue{i}"));

			for (var i = 0; i < 5; ++i)
				requiredProperties.Add(new RequiredPropertyError($"RequiredName{i}"));

			for (var i = 0; i < 8; ++i)
				propertyErrors.Add(new PropertyError($"ErrorName{i}", $"ErrorValue{i}", $"ExpectedType{i}"));
		}

		[Fact]
		public void AddingWarningIncrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection();

			//	Act
			collection.Add(unknownProperty);

			//	Assert 
			Assert.Equal(1, collection.Count);
			Assert.Equal(1, collection.WarningCount);
		}

		[Fact]
		public void AddingMultipleWarningsIncrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection();

			//	Act
			collection.AddRange(unknownProperties);

			//	Assert 
			Assert.Equal(10, collection.Count);
			Assert.Equal(10, collection.WarningCount);
		}

		[Fact]
		public void RemovingWarningDecrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection() { unknownProperty };

			//	Act
			collection.Remove(unknownProperty);

			//	Assert 
			Assert.Equal(0, collection.Count);
			Assert.Equal(0, collection.WarningCount);
		}

		[Fact]
		public void AddingRequiredFieldIncrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection();

			//	Act
			collection.Add(requiredProperty);

			//	Assert 
			Assert.Equal(1, collection.Count);
			Assert.Equal(1, collection.MissingFieldCount);
		}

		[Fact]
		public void AddingMultipleRequiredFieldsIncrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection();

			//	Act
			collection.AddRange(requiredProperties);

			//	Assert 
			Assert.Equal(5, collection.Count);
			Assert.Equal(5, collection.MissingFieldCount);
		}

		[Fact]
		public void RemovingRequiredFieldDecrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection() { requiredProperty };

			//	Act
			collection.Remove(requiredProperty);

			//	Assert 
			Assert.Equal(0, collection.Count);
			Assert.Equal(0, collection.MissingFieldCount);
		}

		[Fact]
		public void AddingPropertyErrorIncrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection();

			//	Act
			collection.Add(propertyError);

			//	Assert 
			Assert.Equal(1, collection.Count);
			Assert.Equal(1, collection.ErrorCount);
		}

		[Fact]
		public void AddingMultiplePropertyErrorsIncrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection();

			//	Act
			collection.AddRange(propertyErrors);

			//	Assert 
			Assert.Equal(8, collection.Count);
			Assert.Equal(8, collection.ErrorCount);
		}

		[Fact]
		public void RemovingPropertyErrorDecrementsCount()
		{
			//	Arrange
			var collection = new PropertyMessageCollection() { propertyError };

			//	Act
			collection.Remove(propertyError);

			//	Assert 
			Assert.Equal(0, collection.Count);
			Assert.Equal(0, collection.ErrorCount);
		}

	}
}
