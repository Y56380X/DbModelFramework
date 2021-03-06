﻿/**
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

using System;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DbModelFramework.Test
{
	[TestClass]
	public class EnumerableContractTest
	{
		class MyTestModel : Model<MyTestModel> { }

		Mock<SqlEngine> sqlEngineMock;

		[TestInitialize]
		public void Init()
		{
			// Setup fakes
			var configuration = new ContainerConfiguration();
			configuration.WithPart<Fakes.DbRequirements>();
			DependencyInjection.InjectionContainer = configuration.CreateContainer();

			// Setup car sqlengine
			sqlEngineMock = new Mock<SqlEngine>() { CallBase = true };
			Fakes.DbRequirements.SqlEngineMock = sqlEngineMock.Object;
		}

		[TestMethod]
		public void CheckIntegerEnumeration()
		{
			var enumerablePropertyInfo = new Mock<PropertyInfo> { CallBase = true, DefaultValue = DefaultValue.Mock };
			var propertyTypeMock = new Mock<Type> { CallBase = true, DefaultValue = DefaultValue.Mock };
			propertyTypeMock.Setup(se => se.GenericTypeArguments).Returns(new[] { typeof(int) });
			enumerablePropertyInfo.Setup(se => se.PropertyType).Returns(propertyTypeMock.Object);

			var enumerableContract = new EnumerableContract(typeof(MyTestModel), enumerablePropertyInfo.Object);

			Assert.AreEqual("int32sTomytestmodels", enumerableContract.tableName);
		}
	}
}
