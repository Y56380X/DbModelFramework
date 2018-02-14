/**
	Copyright (c) 2018 Y56380X

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

		class SingleInt32 : Model<SingleInt32>
		{
			public int MyAttribute { get; set; }
		}

		class SingleInt16 : Model<SingleInt16>
		{
			public short MyAttribute { get; set; }
		}

		class SingleInt64 : Model<SingleInt64>
		{
			public long MyAttribute { get; set; }
		}

		class SingleByteArray : Model<SingleByteArray>
		{
			public byte[] MyBinaryData { get; set; }
		}

		class SingleBoolean : Model<SingleBoolean>
		{
			public bool MyBooleanData { get; set; }
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
	}
}
