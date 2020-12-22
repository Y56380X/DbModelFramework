/*
	Copyright (c) 2018-2020 Y56380X

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
*/

using System.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbModelFramework.Sqlite.Test
{
	[TestClass]
	public class SqlEngineTest
	{
		#region test assets

		class SingleString : Model<SingleString>
		{
			public string MyAttribute { get; set; }
		}

		class MultipleString : Model<MultipleString>
		{
			public string MyAttribute1 { get; set; }
			public string MyAttribute2 { get; set; }
			public string MyAttribute3 { get; set; }
		}

		class MixedType : Model<MixedType>
		{
			public string MyAttribute1 { get; set; }
			public int MyAttribute2 { get; set; }
			public byte[] MyAttribute3 { get; set; }
		}

		class UniqueValue : Model<UniqueValue>
		{
			[DbUnique]
			public string MyAttribute { get; set; }
		}

		class SingleReferencingModel : Model<SingleReferencingModel>
		{
			ReferencedModel MyReference { set; get; }
		}

		class MultipleReferencingModel : Model<MultipleReferencingModel>
		{
			ReferencedModel MyReference1 { set; get; }
			ReferencedModel MyReference2 { set; get; }
		}

		class ReferencedModel : Model<ReferencedModel>
		{
			public string MyAttribute { get; set; }
		}

		class StringPkModel : Model<StringPkModel, string>
		{
			
		}

		#endregion

		[TestInitialize]
		public void Init()
		{
			// Setup fakes
			var configuration = new ContainerConfiguration();
			configuration.WithPart<Fakes.DbRequirements>();

			DependencyInjection.InjectionContainer = configuration.CreateContainer();
		}

		[TestMethod]
		public void CheckDbTableSql()
		{
			var sqlEngine = new SqlEngine();

			var checkTableSql = sqlEngine.CheckTable("models");

			Assert.AreEqual("SELECT name FROM sqlite_master WHERE type='table' AND name='models';",  checkTableSql);
		}

		[TestMethod]
		public void GetLastPrimaryKey()
		{
			var sqlEngine = new SqlEngine();

			var getLastPrimaryKey = sqlEngine.GetLastPrimaryKey();

			Assert.AreEqual("SELECT last_insert_rowid();", getLastPrimaryKey);
		}

		[TestMethod]
		public void CreateTableSql_SingleStringModel()
		{
			var sqlEngine = new SqlEngine();

			var createTableSql = sqlEngine.CreateTable(SingleString.TableName, SingleString.ModelProperties);

			Assert.AreEqual("CREATE TABLE singlestrings (myattribute TEXT, id INTEGER PRIMARY KEY AUTOINCREMENT);", createTableSql);
		}

		[TestMethod]
		public void CreateTableSql_MultipleStringModel()
		{
			var sqlEngine = new SqlEngine();

			var createTableSql = sqlEngine.CreateTable(MultipleString.TableName, MultipleString.ModelProperties);

			Assert.AreEqual("CREATE TABLE multiplestrings (myattribute1 TEXT, myattribute2 TEXT, myattribute3 TEXT, id INTEGER PRIMARY KEY AUTOINCREMENT);", createTableSql);
		}

		[TestMethod]
		public void CreateTableSql_MixedTypeModel()
		{
			var sqlEngine = new SqlEngine();

			var createTableSql = sqlEngine.CreateTable(MixedType.TableName, MixedType.ModelProperties);

			Assert.AreEqual("CREATE TABLE mixedtypes (myattribute1 TEXT, myattribute2 INTEGER, myattribute3 BLOB, id INTEGER PRIMARY KEY AUTOINCREMENT);", createTableSql);
		}

		[TestMethod]
		public void CreateTableSql_UniqueValueModel()
		{
			var sqlEngine = new SqlEngine();

			var createTableSql = sqlEngine.CreateTable(UniqueValue.TableName, UniqueValue.ModelProperties);

			Assert.AreEqual("CREATE TABLE uniquevalues (myattribute TEXT UNIQUE, id INTEGER PRIMARY KEY AUTOINCREMENT);", createTableSql);
		}

		[TestMethod]
		public void CreateTableSql_SingleReferencingModel()
		{
			var sqlEngine = new SqlEngine();

			var createTableSql = sqlEngine.CreateTable(SingleReferencingModel.TableName, SingleReferencingModel.ModelProperties);

			Assert.AreEqual("CREATE TABLE singlereferencingmodels (myreference INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT, FOREIGN KEY(myreference) REFERENCES referencedmodels(id));", createTableSql);
		}

		[TestMethod]
		public void CreateTableSql_MultipleReferencingModel()
		{
			var sqlEngine = new SqlEngine();

			var createTableSql = sqlEngine.CreateTable(MultipleReferencingModel.TableName, MultipleReferencingModel.ModelProperties);

			Assert.AreEqual("CREATE TABLE multiplereferencingmodels (myreference1 INTEGER, myreference2 INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT, "
				+ "FOREIGN KEY(myreference1) REFERENCES referencedmodels(id), FOREIGN KEY(myreference2) REFERENCES referencedmodels(id));", createTableSql);
		}

		[TestMethod]
		public void CreateTableSql_StringPk()
		{
			var sqlEngine = new SqlEngine();

			var createTableSql = sqlEngine.CreateTable(StringPkModel.TableName, StringPkModel.ModelProperties);
			
			Assert.AreEqual("CREATE TABLE stringpkmodels (id TEXT PRIMARY KEY);", createTableSql);
		}

		[TestMethod]
		public void DbTypeToString_Int16()
		{
			Assert.AreEqual("INTEGER", SqlEngine.DbTypeToString(System.Data.DbType.Int16));
		}

		[TestMethod]
		public void DbTypeToString_Int32()
		{
			Assert.AreEqual("INTEGER", SqlEngine.DbTypeToString(System.Data.DbType.Int32));
		}

		[TestMethod]
		public void DbTypeToString_Int64()
		{
			Assert.AreEqual("INTEGER", SqlEngine.DbTypeToString(System.Data.DbType.Int64));
		}

		[TestMethod]
		public void DbTypeToString_String()
		{
			Assert.AreEqual("TEXT", SqlEngine.DbTypeToString(System.Data.DbType.String));
		}

		[TestMethod]
		public void DbTypeToString_Binary()
		{
			Assert.AreEqual("BLOB", SqlEngine.DbTypeToString(System.Data.DbType.Binary));
		}

		[TestMethod]
		public void DbTypeToString_Single() => Assert.AreEqual("REAL", SqlEngine.DbTypeToString(System.Data.DbType.Single));

		[TestMethod]
		public void DbTypeToString_Double() => Assert.AreEqual("REAL", SqlEngine.DbTypeToString(System.Data.DbType.Double));

		[TestMethod]
		public void DbTypeToString_Decimal() => Assert.AreEqual("REAL", SqlEngine.DbTypeToString(System.Data.DbType.Decimal));

		[TestMethod]
		public void DbTypeToString_Date() => Assert.AreEqual("TEXT", SqlEngine.DbTypeToString(System.Data.DbType.DateTime));
	}
}
