﻿/**
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

		#endregion

		[TestMethod]
		public void ToTableCreationSql_SingleString()
		{
			var tableCreationSql =  SingleString.ModelProperties.ToTableCreationSql();
			
			Assert.AreEqual("myattribute TEXT, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		}

		[TestMethod]
		public void ToTableCreationSql_MultipleString()
		{
			var tableCreationSql = MultipleString.ModelProperties.ToTableCreationSql();

			Assert.AreEqual("myattribute1 TEXT, myattribute2 TEXT, myattribute3 TEXT, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		}

		[TestMethod]
		public void ToAttributeChainSqlWithPK_SingleString()
		{
			var insertAttributesSql = SingleString.ModelProperties.ToAttributeChainSql(true);

			Assert.AreEqual("myattribute, id", insertAttributesSql);
		}

		[TestMethod]
		public void ToAttributeChainSqlWithPK_MultipleString()
		{
			var insertAttributesSql = MultipleString.ModelProperties.ToAttributeChainSql(true);

			Assert.AreEqual("myattribute1, myattribute2, myattribute3, id", insertAttributesSql);
		}

		[TestMethod]
		public void ToAttributeChainSqlWithoutPK_SingleString()
		{
			var insertAttributesSql = SingleString.ModelProperties.ToAttributeChainSql();

			Assert.AreEqual("myattribute", insertAttributesSql);
		}

		[TestMethod]
		public void ToAttributeChainSqlWithoutPK_MultipleString()
		{
			var insertAttributesSql = MultipleString.ModelProperties.ToAttributeChainSql();

			Assert.AreEqual("myattribute1, myattribute2, myattribute3", insertAttributesSql);
		}

		[TestMethod]
		public void ToInsertParameterChainSql_SingleString()
		{
			var insertParametersSql = SingleString.ModelProperties.ToInsertParameterChainSql();

			Assert.AreEqual("@myattribute", insertParametersSql);
		}

		[TestMethod]
		public void ToInsertParameterChainSql_MultipleString()
		{
			var insertParametersSql = MultipleString.ModelProperties.ToInsertParameterChainSql();

			Assert.AreEqual("@myattribute1, @myattribute2, @myattribute3", insertParametersSql);
		}

		[TestMethod]
		public void ToTableCreationSql_SingleInt32()
		{
			var tableCreationSql = SingleInt32.ModelProperties.ToTableCreationSql();

			Assert.AreEqual("myattribute INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		}

		[TestMethod]
		public void ToTableCreationSql_SingleInt16()
		{
			var tableCreationSql = SingleInt16.ModelProperties.ToTableCreationSql();

			Assert.AreEqual("myattribute INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		}

		[TestMethod]
		public void ToTableCreationSql_SingleInt64()
		{
			var tableCreationSql = SingleInt64.ModelProperties.ToTableCreationSql();

			Assert.AreEqual("myattribute INTEGER, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		}

		[TestMethod]
		public void ToUpdateSql_SingleString()
		{
			var updateSql = SingleString.ModelProperties.ToUpdateSql();

			Assert.AreEqual("myattribute = @myattribute", updateSql);
		}

		[TestMethod]
		public void ToUpdateSql_MultipleString()
		{
			var updateSql = MultipleString.ModelProperties.ToUpdateSql();

			Assert.AreEqual("myattribute1 = @myattribute1, myattribute2 = @myattribute2, myattribute3 = @myattribute3", updateSql);
		}

		[TestMethod]
		public void ToTableCreationString_UniqueValues()
		{
			var tableCreationSql = UniqueValue.ModelProperties.ToTableCreationSql();

			Assert.AreEqual("myattribute TEXT UNIQUE, id INTEGER PRIMARY KEY AUTOINCREMENT", tableCreationSql);
		}

		[TestMethod]
		public void ToWhereSql_EqualsStringValue()
		{
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };

			var toWhereSql = SqlExtenstions.ToWhereSql((Expression<Func<CustomExpression, bool>>)(model => model.MyStringAttribute == "Value"), dbCommandMock.Object);

			Assert.AreEqual("mystringattribute = @mystringattribute", toWhereSql);
		}
	}
}
