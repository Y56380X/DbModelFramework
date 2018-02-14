/**
	Copyright (c) 2017-2018 Y56380X

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
using System.Composition.Hosting;
using System.Data;
using System.Linq;
using static DbModelFramework.Test.ModelTest.SqlFakes;

namespace DbModelFramework.Test
{
	[TestClass]
	public class ModelTest
	{
		#region test assets

		public static class SqlFakes
		{
			public static string Car_CheckTableSql = "CHECK TABLE CARS;";
			public static string Car_CreateTableSql = "CREATE TABLE CARS;";

			public static string Build_CheckTableSql = "CHECK TABLE BUILDS;";
			public static string Build_CreateTableSql = "CREATE TABLE BUILDS;";

			public static string Product_CheckTableSql = "CHECK TABLE PRODUCTS;";
			public static string Product_CreateTableSql = "CREATE TABLE PRODUCTS;";

			public static string Manufacturer_CheckTableSql = "CHECK TABLE MANUFACTURERS;";
			public static string Manufacturer_CreateTableSql = "CREATE TABLE MANUFACTURERS;";
		}

		class Car : Model<Car>
		{
			public string Manufacturer { get; set; }
			public string Type { get; set; }

			[DbIgnore]
			public new long Id => base.Id;
		}

		class Build : Model<Build>
		{
			public string Name { get; set; }

			public byte[] Artifact { get; set; }
		}

		class Product : Model<Product>
		{
			public string Name { get; set; }

			public Manufacturer Manufacturer { get; set; }
		}

		class Manufacturer : Model<Manufacturer>
		{
			[DbIgnore]
			public new long Id => base.Id;

			public string Name { get; set; }
		}

		#endregion
	
		[TestInitialize]
		public void Init()
		{
			// Setup fakes
			var configuration = new ContainerConfiguration();
			configuration.WithPart<Fakes.DbRequirements>();

			// Setup car sqlengine
			var sqlEngineMock = new Mock<SqlEngine>() { CallBase = true };
			sqlEngineMock.Setup(se => se.CheckTable("cars")).Returns(Car_CheckTableSql);
			sqlEngineMock.Setup(se => se.CreateTable("cars", Car.ModelProperties)).Returns(Car_CreateTableSql);
			sqlEngineMock.Setup(se => se.CheckTable("builds")).Returns(Build_CheckTableSql);
			sqlEngineMock.Setup(se => se.CreateTable("builds", Build.ModelProperties)).Returns(Build_CreateTableSql);
			sqlEngineMock.Setup(se => se.CheckTable("products")).Returns(Product_CheckTableSql);
			sqlEngineMock.Setup(se => se.CreateTable("products", Product.ModelProperties)).Returns(Product_CreateTableSql);
			sqlEngineMock.Setup(se => se.CheckTable("manufacturers")).Returns(Manufacturer_CheckTableSql);
			sqlEngineMock.Setup(se => se.CreateTable("manufacturers", Manufacturer.ModelProperties)).Returns(Manufacturer_CheckTableSql);
			sqlEngineMock.Setup(se => se.DeleteModel()).Returns(string.Empty);
			sqlEngineMock.Setup(se => se.SelectModel()).Returns(string.Empty);
			sqlEngineMock.Setup(se => se.UpdateModel()).Returns(string.Empty);
			Fakes.DbRequirements.SqlEngineMock = sqlEngineMock.Object;

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
			var propertyNames = Car.ModelProperties.Select(prop => prop.PropertyName);

			Assert.IsTrue(propertyNames.Contains("Manufacturer"));
			Assert.IsTrue(propertyNames.Contains("Type"));
		}

		[TestMethod]
		public void CheckTable()
		{
			var checkTableSql = Car.Sql.CheckTable;

			Assert.AreEqual(Car_CheckTableSql, checkTableSql);
			Assert.IsTrue(Fakes.DbConnection.CreatedCommands.Select(c => c.CommandText).Contains(checkTableSql));
		}

		[TestMethod]
		public void CreateTable()
		{
			var createTable = Car.Sql.CreateTable;

			Assert.AreEqual(Car_CreateTableSql, createTable);
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
			Fakes.DbConnection.CreatedCommands.Clear();

			var car = Car.Create();

			Assert.IsNotNull(car);
			var command = Fakes.DbConnection.CreatedCommands.SingleOrDefault(c => c.CommandText == Car.Sql.Insert);
			Assert.IsNotNull(command);
			Assert.IsTrue(command.Parameters.Contains("@manufacturer"));
			Assert.IsTrue(command.Parameters.Contains("@type"));
		}

		[TestMethod]
		public void CreateNewModelInstanceShouldUpdatePK()
		{
			Fakes.DbConnection.CreatedCommands.Clear();
			Fakes.DbConnection.ClearCustomExecuteResults();
			Fakes.DbConnection.AddCustomExecuteScalarResult(Car.Sql.LastPrimaryKey, 1);

			var car = Car.Create();

			Assert.IsNotNull(car);
			Assert.AreEqual(1, car.Id);
			var command = Fakes.DbConnection.CreatedCommands.SingleOrDefault(c => c.CommandText == Car.Sql.LastPrimaryKey);
			Assert.IsNotNull(command);
		}

		[TestMethod]
		public void GetAllDataAsModelInstancesFromDb()
		{
			var dataReaderMock = new Mock<IDataReader>();
			int counter = 0;
			dataReaderMock.Setup(dr => dr.Read()).Returns(() => { return counter++ < 3; });

			Fakes.DbConnection.ClearCustomExecuteResults();
			Fakes.DbConnection.AddCustomExecuteReaderResult("SELECT manufacturer, type, id FROM cars;", dataReaderMock.Object);

			var cars = Car.Get();

			Assert.IsNotNull(cars);
			Assert.AreEqual(3, cars.Count());
		}

		[TestMethod]
		public void GetSingleDataAsModelInstanceFromDbByPrimaryKey()
		{
			var dataReaderMock = new Mock<IDataReader>();
			int counter = 0;
			dataReaderMock.Setup(dr => dr.Read()).Returns(() => { return counter++ < 1; });
			dataReaderMock.Setup(dr => dr["id"]).Returns(3);

			Fakes.DbConnection.ClearCustomExecuteResults();
			Fakes.DbConnection.AddCustomExecuteReaderResult("SELECT manufacturer, type, id FROM cars WHERE id = @id;", dataReaderMock.Object);

			var car = Car.Get(3);

			Assert.IsNotNull(car);
			Assert.AreEqual(3, car.Id);
		}

		[TestMethod]
		public void DeleteModelSql()
		{
			var deleteModel = Car.Sql.Delete;

			Assert.AreEqual("DELETE FROM cars WHERE id = @id;", deleteModel);
		}

		[TestMethod]
		public void DeleteModelFromDb()
		{
			var car = Car.Create();

			car.Delete();

			var command = Fakes.DbConnection.CreatedCommands.SingleOrDefault(c => c.CommandText == Car.Sql.Delete);
			Assert.IsNotNull(command);
			Assert.IsTrue(command.Parameters.Contains("@id"));
		}

		[TestMethod]
		public void UpdateModelSql()
		{
			var updateCar = Car.Sql.Update;

			Assert.AreEqual("UPDATE cars SET manufacturer = @manufacturer, type = @type WHERE id = @id;", updateCar);
		}

		[TestMethod]
		public void SelectModelByPrimaryKeySql()
		{
			var selectSingleModelByPk = Car.Sql.SelectByPrimaryKey;

			Assert.AreEqual("SELECT manufacturer, type, id FROM cars WHERE id = @id;", selectSingleModelByPk);
		}

		[TestMethod]
		public void ReloadModelData_ChangesShouldDiscardChanges()
		{
			var dataReaderMock = new Mock<IDataReader>();

			dataReaderMock.Setup(dr => dr.Read()).Returns(true);
			dataReaderMock.Setup(dr => dr["id"]).Returns(1);
			dataReaderMock.Setup(dr => dr["manufacturer"]).Returns("ImaginaryManufacturer");

			Fakes.DbConnection.ClearCustomExecuteResults();
			Fakes.DbConnection.AddCustomExecuteReaderResult("SELECT manufacturer, type, id FROM cars WHERE id = @id;", dataReaderMock.Object);

			var car = Car.Get(1);

			car.Manufacturer = "TheMagicManufacturer";

			car.Reload();

			Assert.AreEqual("ImaginaryManufacturer", car.Manufacturer);
		}

		[TestMethod]
		public void CreateModelWithValues()
		{
			Fakes.DbConnection.CreatedCommands.Clear();

			var car = Car.Create(c =>
			{
				c.Manufacturer = "TheMagicManufacturer";
				c.Type = "Sedan";
			});
			
			var command = Fakes.DbConnection.CreatedCommands.SingleOrDefault(c => c.CommandText == Car.Sql.Insert);

			Assert.AreEqual("TheMagicManufacturer", (command.Parameters["@manufacturer"] as IDbDataParameter).Value);
			Assert.AreEqual("Sedan", (command.Parameters["@type"] as IDbDataParameter).Value);
		}

		[TestMethod]
		public void GetModelWithBinaryProperty()
		{
			var buildArtifact = new byte[] { 10, 20, 30, 40 };

			var dataReaderMock = new Mock<IDataReader>();

			dataReaderMock.Setup(dr => dr.Read()).Returns(true);
			dataReaderMock.Setup(dr => dr["id"]).Returns(1);
			dataReaderMock.Setup(dr => dr["artifact"]).Returns(buildArtifact);

			Fakes.DbConnection.ClearCustomExecuteResults();
			Fakes.DbConnection.AddCustomExecuteReaderResult("SELECT name, artifact, id FROM builds WHERE id = @id;", dataReaderMock.Object);

			var build = Build.Get(1);

			CollectionAssert.AreEqual(buildArtifact, build.Artifact);
		}

		[TestMethod]
		public void CreateModelWithReferencedModel_ReferencedModelHasPk()
		{
			Fakes.DbConnection.CreatedCommands.Clear();
			Fakes.DbConnection.ClearCustomExecuteResults();
			Fakes.DbConnection.AddCustomExecuteScalarResult(Product.Sql.LastPrimaryKey, 1);

			var product = Product.Create(p =>
			{
				p.Manufacturer = Manufacturer.Create();
				p.Name = "My imaginary product";
			});

			Assert.AreEqual(1, product.Manufacturer.Id);
		}

		[TestMethod]
		public void GetModelWithReferencedModel_IsReferenceInstanciated()
		{
			var productDataReaderMock = new Mock<IDataReader>();
			productDataReaderMock.Setup(dr => dr.Read()).Returns(true);
			productDataReaderMock.Setup(dr => dr["id"]).Returns(1);
			productDataReaderMock.Setup(dr => dr["manufacturer"]).Returns(1);
			productDataReaderMock.Setup(dr => dr["name"]).Returns("My imaginary product");

			var manufacturerDataReaderMock = new Mock<IDataReader>();
			manufacturerDataReaderMock.Setup(dr => dr.Read()).Returns(true);
			manufacturerDataReaderMock.Setup(dr => dr["id"]).Returns(1);
			manufacturerDataReaderMock.Setup(dr => dr["name"]).Returns("My imaginary manufacturer");

			Fakes.DbConnection.ClearCustomExecuteResults();
			Fakes.DbConnection.AddCustomExecuteReaderResult(Product.Sql.SelectByPrimaryKey, productDataReaderMock.Object);
			Fakes.DbConnection.AddCustomExecuteReaderResult(Manufacturer.Sql.SelectByPrimaryKey, manufacturerDataReaderMock.Object);

			var product = Product.Get(1);

			Assert.IsNotNull(product.Manufacturer);
			Assert.AreEqual("My imaginary product", product.Name);
			Assert.AreEqual("My imaginary manufacturer", product.Manufacturer.Name);
		}
	}
}
