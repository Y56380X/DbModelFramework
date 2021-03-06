﻿/**
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

using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DbModelFramework.Test
{
	[TestClass]
	public class ModelPropertyTest
	{
		#region test assets

		class TestModel : Model<TestModel>
		{
			public long MyAttribute1 { get; set; }
			public int MyAttribute2 { get; set; }
			public short MyAttribute3 { get; set; }
		}


		class ModelWithReferencedModel : Model<ModelWithReferencedModel>
		{
			public ReferenceModel MyReferencedModel { get; set; }
		}

		class ReferenceModel : Model<ReferenceModel>
		{
		}

		class ModelWithEnumerationOfValues : Model<ModelWithEnumerationOfValues>
		{
			IEnumerable<int> MyEnumeration { get; set; }
		}

		class ModelWithEnumerationOfReferences : Model<ModelWithEnumerationOfReferences>
		{
			IEnumerable<ReferenceModel> MyEnumeration { get; set; }
		}

		enum TestEnum
		{
			Test1,
			Test2
		}

		#endregion

		[TestInitialize]
		public void Init()
		{
			// Setup fakes
			var configuration = new ContainerConfiguration();
			configuration.WithPart<Fakes.DbConnection>();
			configuration.WithPart<Fakes.DbRequirements>();

			DependencyInjection.InjectionContainer = configuration.CreateContainer();
		}

		[TestMethod]
		public void SetValueInt64()
		{
			var modelProperty = new ModelProperty(typeof(TestModel).GetProperty("MyAttribute1"));
			var model = new TestModel();

			modelProperty.SetValue(model, (long)1200);

			Assert.AreEqual(1200, model.MyAttribute1);
		}

		[TestMethod]
		public void SetValueInt32()
		{
			var modelProperty = new ModelProperty(typeof(TestModel).GetProperty("MyAttribute2"));
			var model = new TestModel();

			modelProperty.SetValue(model, (long)1200);

			Assert.AreEqual(1200, model.MyAttribute2);
		}

		[TestMethod]
		public void SetValueInt16()
		{
			var modelProperty = new ModelProperty(typeof(TestModel).GetProperty("MyAttribute3"));
			var model = new TestModel();

			modelProperty.SetValue(model, (long)1200);

			Assert.AreEqual(1200, model.MyAttribute3);
		}

		[TestMethod]
		public void GetForeignKey_IsNotNull()
		{
			var foreignKey = ModelWithReferencedModel.ModelProperties.SingleOrDefault(prop => prop.IsForeignKey);

			Assert.IsNotNull(foreignKey);
		}

		[TestMethod]
		public void GetForeignKey_HasCorrectName()
		{
			var foreignKey = ModelWithReferencedModel.ModelProperties.SingleOrDefault(prop => prop.IsForeignKey);

			Assert.AreEqual(nameof(ModelWithReferencedModel.MyReferencedModel), foreignKey.PropertyName);
		}

		[TestMethod]
		public void GetForeignKey_HasCorrectType()
		{
			var foreignKey = ModelWithReferencedModel.ModelProperties.SingleOrDefault(prop => prop.IsForeignKey);

			Assert.AreEqual(ReferenceModel.PrimaryKeyProperty.Type, foreignKey.Type);
		}

		[TestMethod]
		public void GetForeignKey_ForeignKeyRelationIsReferencedPk()
		{
			var foreignKey = ModelWithReferencedModel.ModelProperties.SingleOrDefault(prop => prop.IsForeignKey);

			Assert.IsNotNull(foreignKey.ForeignKeyReference);
			Assert.AreEqual(ReferenceModel.PrimaryKeyProperty, foreignKey.ForeignKeyReference);
		}

		[TestMethod]
		public void ModelPropertyFromEnumType()
		{
			var enumPropertyMock = new Mock<System.Reflection.PropertyInfo>();
			enumPropertyMock.Setup(pi => pi.PropertyType).Returns(typeof(TestEnum));
			enumPropertyMock.Setup(pi => pi.Name).Returns("Enum");

			var modelProperty = new ModelProperty(enumPropertyMock.Object);
		}
	}
}
