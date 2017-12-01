/**
	Copyright (c) 2017 Y56380X

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
**/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Data;
using System.Linq;

namespace DbModelFramework.Test
{
	[TestClass]
	public class ModelTest
	{
		#region test assets

		class Car : Model<Car>
		{
			public string Manufacturer { get; set; }
			public string Type { get; set; }
		}

		#endregion
	
		[TestInitialize]
		public void Init()
		{
			// Setup fake db connection
			var configuration = new ContainerConfiguration();
			configuration.WithPart<Fakes.DbConnection>();

			DependencyInjection.InjectionContainer = configuration.CreateContainer();
		}

		[TestMethod]
		public void TableNameOfModelShouldBeModelsNameWithS()
		{
			Assert.AreEqual("cars", Car.TableName);
		}

		[TestMethod]
		public void PropertiesOfModel()
		{
			var propertyNames = Car.ModelProperties.Select(prop => prop.Name);

			Assert.IsTrue(propertyNames.Contains("Manufacturer"));
			Assert.IsTrue(propertyNames.Contains("Type"));
		}

		[TestMethod]
		public void CheckTable()
		{
			var checkTable = Car.Sql.CheckTable;

			Assert.AreEqual("SELECT name FROM sqlite_master WHERE type='table' AND name='cars';", checkTable);
			Assert.IsTrue(Fakes.DbConnection.CreatedCommands.Select(c => c.CommandText).Contains(checkTable));
		}

		[TestMethod]
		public void CreateTable()
		{
			var createTable = Car.Sql.CreateTable;

			Assert.AreEqual("CREATE TABLE cars (manufacturer String, type String);", createTable);
			Assert.IsTrue(Fakes.DbConnection.CreatedCommands.Select(c => c.CommandText).Contains(createTable));
		}

		[TestMethod]
		public void InsertIntoTable()
		{
			var insert = Car.Sql.Insert;

			Assert.AreEqual("INSERT INTO cars (manufacturer, type) VALUES (@manufacturer, @type);", insert);
		}

		[TestMethod]
		public void CreateNewModelInstanceShouldInsertInDb()
		{
			var car = Car.Create();

			Assert.IsNotNull(car);
			var command = Fakes.DbConnection.CreatedCommands.SingleOrDefault(c => c.CommandText == Car.Sql.Insert);
			Assert.IsNotNull(command);
			Assert.IsTrue(command.Parameters.Contains("@manufacturer"));
			Assert.IsTrue(command.Parameters.Contains("@type"));
		}

		[TestMethod]
		public void GetAllDataAsModelInstancesFromDb()
		{
			var dataReaderMock = new Mock<IDataReader>();
			int counter = 0;
			dataReaderMock.Setup(dr => dr.Read()).Returns(() => { return counter++ < 3; });

			Fakes.DbConnection.AddCustomExecuteReaderResult("SELECT manufacturer, type FROM cars;", dataReaderMock.Object);

			var cars = Car.Get();

			Assert.IsNotNull(cars);
			Assert.AreEqual(3, cars.Count());
		}
	}
}
