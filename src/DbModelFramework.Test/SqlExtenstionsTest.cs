/**
	Copyright (c) 2017-2020 Y56380X

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
		
		class CustomExpression : Model<CustomExpression>
		{
			public string MyStringAttribute { get; set; }

			public int MyIntAttribute { get; set; }
		}

		class CascadedFilter
		{
			public CustomExpression CustomExpressionFilter { get; set; }
		}

		enum Int16TestEnum : Int16
		{
			Test1,
			Test2
		}

		enum Int32TestEnum : Int32
		{
			Test1,
			Test2
		}

		enum Int64TestEnum : Int64
		{
			Test1,
			Test2
		}

		#endregion

		[TestMethod]
		public void ToWhereSql_EqualsStringValue_SqlIsCorrect()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			var whereSql = SqlExtension.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == "Value"), dbCommandMock.Object);

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

			SqlExtension.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == "Value"), dbCommand);

			Assert.IsTrue(dbCommand.Parameters.Contains("@mystringattribute"));
			Assert.AreEqual("Value", (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).Value);
			Assert.AreEqual(DbType.String, (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).DbType);
		}

		[TestMethod]
		public void ToWhereSql_EqualsFieldValue_SqlIsCorrect()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			string value = "Value";
			var whereSql = SqlExtension.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == value), dbCommandMock.Object);

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
			SqlExtension.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == value), dbCommand);

			Assert.IsTrue(dbCommand.Parameters.Contains("@mystringattribute"));
			Assert.AreEqual("Value", (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).Value);
			Assert.AreEqual(DbType.String, (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).DbType);
		}

		[TestMethod]
		public void ToWhereSql_EqualsPropertyValue_SqlIsCorrect()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			var filter = new CustomExpression { MyStringAttribute = "Value" };
			var whereSql = SqlExtension.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == filter.MyStringAttribute), dbCommandMock.Object);

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
			SqlExtension.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == filter.MyStringAttribute), dbCommand);

			Assert.IsTrue(dbCommand.Parameters.Contains("@mystringattribute"));
			Assert.AreEqual("Value", (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).Value);
			Assert.AreEqual(DbType.String, (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).DbType);
		}

		[TestMethod]
		public void ToWhereSql_EqualsNestedPropertyValue_SqlIsCorrect()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			var filter = new CascadedFilter { CustomExpressionFilter = new CustomExpression { MyStringAttribute = "Value" } };
			var whereSql = SqlExtension.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == filter.CustomExpressionFilter.MyStringAttribute), dbCommandMock.Object);

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
			SqlExtension.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == filter.CustomExpressionFilter.MyStringAttribute), dbCommand);

			Assert.IsTrue(dbCommand.Parameters.Contains("@mystringattribute"));
			Assert.AreEqual("Value", (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).Value);
			Assert.AreEqual(DbType.String, (dbCommand.Parameters["@mystringattribute"] as IDbDataParameter).DbType);
		}

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

		[TestMethod]
		public void ToDbType_Boolean()
		{
			Assert.AreEqual(DbType.Boolean, typeof(bool).ToDbType());
		}

		[TestMethod]
		public void ToDbType_Int16TestEnum()
		{
			Assert.AreEqual(DbType.Int16, typeof(Int16TestEnum).ToDbType());
		}

		[TestMethod]
		public void ToDbType_Int32TestEnum()
		{
			Assert.AreEqual(DbType.Int32, typeof(Int32TestEnum).ToDbType());
		}

		[TestMethod]
		public void ToDbType_Int64TestEnum()
		{
			Assert.AreEqual(DbType.Int64, typeof(Int64TestEnum).ToDbType());
		}

		[TestMethod]
		public void ToDbType_Float() => Assert.AreEqual(DbType.Single, typeof(float).ToDbType());

		[TestMethod]
		public void ToDbType_Double() => Assert.AreEqual(DbType.Double, typeof(double).ToDbType());

		[TestMethod]
		public void ToDbType_Decimal() => Assert.AreEqual(DbType.Decimal, typeof(decimal).ToDbType());

		[TestMethod]
		public void ToDbType_DateTime() => Assert.AreEqual(DbType.DateTime, typeof(DateTime).ToDbType());
	}
}
