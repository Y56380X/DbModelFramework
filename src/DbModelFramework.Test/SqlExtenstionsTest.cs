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
using System;
using System.Data;
using System.Linq.Expressions;

namespace DbModelFramework.Test
{
	[TestClass]
	public class SqlExtenstionsTest
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

		class UniqueValue : Model<UniqueValue>
		{
			[DbUnique]
			public string MyAttribute { get; set; }
		}

		class CustomExpression : Model<CustomExpression>
		{
			public string MyStringAttribute { get; set; }

			public int MyIntAttribute { get; set; }
		}

		class CascadedFilter
		{
			public CustomExpression CustomExpressionFilter { get; set; }
		}

		class ModelWithReferencedModel : Model<ModelWithReferencedModel>
		{
			public string MyAttribute { get; set; }

			public ReferenceModel MyReferencedModel { get; set; }
		}

		class ReferenceModel : Model<ReferenceModel>
		{
			public string MyAttribute { get; set; }
		}

		#endregion

		//[TestMethod]
		//public void ToTableCreationSql_SingleString()
		//{
		//	var tableCreationSql =  SingleString.ModelProperties.ToTableCreationSql();
			
		//	Assert.AreEqual("myattribute TEXT, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		//}

		//[TestMethod]
		//public void ToTableCreationSql_MultipleString()
		//{
		//	var tableCreationSql = MultipleString.ModelProperties.ToTableCreationSql();

		//	Assert.AreEqual("myattribute1 TEXT, myattribute2 TEXT, myattribute3 TEXT, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		//}

		//[TestMethod]
		//public void ToAttributeChainSqlWithPK_SingleString()
		//{
		//	var insertAttributesSql = SingleString.ModelProperties.ToAttributeChainSql(true);

		//	Assert.AreEqual("myattribute, id", insertAttributesSql);
		//}

		//[TestMethod]
		//public void ToAttributeChainSqlWithPK_MultipleString()
		//{
		//	var insertAttributesSql = MultipleString.ModelProperties.ToAttributeChainSql(true);

		//	Assert.AreEqual("myattribute1, myattribute2, myattribute3, id", insertAttributesSql);
		//}

		//[TestMethod]
		//public void ToAttributeChainSqlWithoutPK_SingleString()
		//{
		//	var insertAttributesSql = SingleString.ModelProperties.ToAttributeChainSql();

		//	Assert.AreEqual("myattribute", insertAttributesSql);
		//}

		//[TestMethod]
		//public void ToAttributeChainSqlWithoutPK_MultipleString()
		//{
		//	var insertAttributesSql = MultipleString.ModelProperties.ToAttributeChainSql();

		//	Assert.AreEqual("myattribute1, myattribute2, myattribute3", insertAttributesSql);
		//}

		//[TestMethod]
		//public void ToInsertParameterChainSql_SingleString()
		//{
		//	var insertParametersSql = SingleString.ModelProperties.ToInsertParameterChainSql();

		//	Assert.AreEqual("@myattribute", insertParametersSql);
		//}

		//[TestMethod]
		//public void ToInsertParameterChainSql_MultipleString()
		//{
		//	var insertParametersSql = MultipleString.ModelProperties.ToInsertParameterChainSql();

		//	Assert.AreEqual("@myattribute1, @myattribute2, @myattribute3", insertParametersSql);
		//}

		//[TestMethod]
		//public void ToTableCreationSql_SingleInt32()
		//{
		//	var tableCreationSql = SingleInt32.ModelProperties.ToTableCreationSql();

		//	Assert.AreEqual("myattribute INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		//}

		//[TestMethod]
		//public void ToTableCreationSql_SingleInt16()
		//{
		//	var tableCreationSql = SingleInt16.ModelProperties.ToTableCreationSql();

		//	Assert.AreEqual("myattribute INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		//}

		//[TestMethod]
		//public void ToTableCreationSql_SingleInt64()
		//{
		//	var tableCreationSql = SingleInt64.ModelProperties.ToTableCreationSql();

		//	Assert.AreEqual("myattribute INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		//}

		//[TestMethod]
		//public void ToUpdateSql_SingleString()
		//{
		//	var updateSql = SingleString.ModelProperties.ToUpdateSql();

		//	Assert.AreEqual("myattribute = @myattribute", updateSql);
		//}

		//[TestMethod]
		//public void ToUpdateSql_MultipleString()
		//{
		//	var updateSql = MultipleString.ModelProperties.ToUpdateSql();

		//	Assert.AreEqual("myattribute1 = @myattribute1, myattribute2 = @myattribute2, myattribute3 = @myattribute3", updateSql);
		//}

		//[TestMethod]
		//public void ToTableCreationString_UniqueValues()
		//{
		//	var tableCreationSql = UniqueValue.ModelProperties.ToTableCreationSql();

		//	Assert.AreEqual("myattribute TEXT UNIQUE, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		//}

		[TestMethod]
		public void ToWhereSql_EqualsStringValue_SqlIsCorrect()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			var whereSql = SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == "Value"), dbCommandMock.Object);

			Assert.AreEqual("mystringattribute = @mystringattribute", whereSql);
		}

		[TestMethod]
		public void ToWhereSql_EqualsStringValue_HasParameter()
		{
			var parameters = new Fakes.DataParameterCollection();
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };
			dbCommandMock.SetupGet(command => command.Parameters).Returns(parameters);
			dbCommandMock.Setup(command => command.CreateParameter()).Returns(() => Mock.Of<IDbDataParameter>());
			var dbCommand = dbCommandMock.Object;

			SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == "Value"), dbCommand);

			Assert.IsTrue(dbCommand.Parameters.Contains("@mystringattribute"));
			Assert.AreEqual("Value", (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).Value);
			Assert.AreEqual(DbType.String, (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).DbType);
		}

		[TestMethod]
		public void ToWhereSql_EqualsFieldValue_SqlIsCorrect()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			string value = "Value";
			var whereSql = SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == value), dbCommandMock.Object);

			Assert.AreEqual("mystringattribute = @mystringattribute", whereSql);
		}

		[TestMethod]
		public void ToWhereSql_EqualsFieldValue_HasParameter()
		{
			var parameters = new Fakes.DataParameterCollection();
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };
			dbCommandMock.SetupGet(command => command.Parameters).Returns(parameters);
			dbCommandMock.Setup(command => command.CreateParameter()).Returns(() => Mock.Of<IDbDataParameter>());
			var dbCommand = dbCommandMock.Object;

			string value = "Value";
			SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == value), dbCommand);

			Assert.IsTrue(dbCommand.Parameters.Contains("@mystringattribute"));
			Assert.AreEqual("Value", (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).Value);
			Assert.AreEqual(DbType.String, (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).DbType);
		}

		[TestMethod]
		public void ToWhereSql_EqualsPropertyValue_SqlIsCorrect()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			var filter = new CustomExpression { MyStringAttribute = "Value" };
			var whereSql = SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == filter.MyStringAttribute), dbCommandMock.Object);

			Assert.AreEqual("mystringattribute = @mystringattribute", whereSql);
		}

		[TestMethod]
		public void ToWhereSql_EqualsPropertyValue_HasParameter()
		{
			var parameters = new Fakes.DataParameterCollection();
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };
			dbCommandMock.SetupGet(command => command.Parameters).Returns(parameters);
			dbCommandMock.Setup(command => command.CreateParameter()).Returns(() => Mock.Of<IDbDataParameter>());
			var dbCommand = dbCommandMock.Object;

			var filter = new CustomExpression { MyStringAttribute = "Value" };
			SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == filter.MyStringAttribute), dbCommand);

			Assert.IsTrue(dbCommand.Parameters.Contains("@mystringattribute"));
			Assert.AreEqual("Value", (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).Value);
			Assert.AreEqual(DbType.String, (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).DbType);
		}

		[TestMethod]
		public void ToWhereSql_EqualsNestedPropertyValue_SqlIsCorrect()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			var filter = new CascadedFilter { CustomExpressionFilter = new CustomExpression { MyStringAttribute = "Value" } };
			var whereSql = SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == filter.CustomExpressionFilter.MyStringAttribute), dbCommandMock.Object);

			Assert.AreEqual("mystringattribute = @mystringattribute", whereSql);
		}

		[TestMethod]
		public void ToWhereSql_EqualsNestedPropertyValue_HasParameter()
		{
			var parameters = new Fakes.DataParameterCollection();
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };
			dbCommandMock.SetupGet(command => command.Parameters).Returns(parameters);
			dbCommandMock.Setup(command => command.CreateParameter()).Returns(() => Mock.Of<IDbDataParameter>());
			var dbCommand = dbCommandMock.Object;

			var filter = new CascadedFilter { CustomExpressionFilter = new CustomExpression { MyStringAttribute = "Value" } };
			SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == filter.CustomExpressionFilter.MyStringAttribute), dbCommand);

			Assert.IsTrue(dbCommand.Parameters.Contains("@mystringattribute"));
			Assert.AreEqual("Value", (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).Value);
			Assert.AreEqual(DbType.String, (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).DbType);
		}

		//[TestMethod]
		//public void ToTableCreationSql_SingleByteArray()
		//{
		//	var tableCreationSql = SingleByteArray.ModelProperties.ToTableCreationSql();

		//	Assert.AreEqual("mybinarydata BLOB, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		//}

		//[TestMethod]
		//public void ToTableCreationSql_ModelWithReferencedModel()
		//{
		//	var createTableSql = ModelWithReferencedModel.ModelProperties.ToTableCreationSql();

		//	Assert.AreEqual("myattribute TEXT, myreferencedmodel INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT, FOREIGN KEY(myreferencedmodel) REFERENCES referencemodels(id)", createTableSql);
		//}

		[TestMethod]
		public void ToDbType_Int32()
		{
			Assert.AreEqual(DbType.Int32, typeof(int).ToDbType());
		}

		[TestMethod]
		public void ToDbType_Int16()
		{
			Assert.AreEqual(DbType.Int16, typeof(short).ToDbType());
		}

		[TestMethod]
		public void ToDbType_Int64()
		{
			Assert.AreEqual(DbType.Int64, typeof(long).ToDbType());
		}

		[TestMethod]
		public void ToDbType_String()
		{
			Assert.AreEqual(DbType.String, typeof(string).ToDbType());
		}

		[TestMethod]
		public void ToDbType_ByteArray()
		{
			Assert.AreEqual(DbType.Binary, typeof(byte[]).ToDbType());
		}
	}
}
