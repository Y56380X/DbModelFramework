/**
	Copyright (c) 2020 Y56380X

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
using System.Data;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DbModelFramework.Test
{
	[TestClass]
	public class CustomWhereWithModelsTest
	{
		#region Test Assets

		class ModelOne : Model<ModelOne>
		{
			public ModelReferenced Reference { get; set; }
		}

		public class ModelReferenced : Model<ModelReferenced>
		{
			public new virtual long Id
			{
				get => base.Id;
				set => base.Id = value;
			}
		}
		
		#endregion

		[TestInitialize]
		public void Init()
		{
			// Setup fakes
			var configuration = new ContainerConfiguration();
			configuration.WithPart<Fakes.DbRequirements>();
			DependencyInjection.InjectionContainer = configuration.CreateContainer();

			// Setup car sqlengine
			var sqlEngineMock = new Mock<SqlEngine>() { CallBase = true };
			Fakes.DbRequirements.SqlEngineMock = sqlEngineMock.Object;
		}
		
		[TestMethod]
		public void ToWhereSql_contains_Fk_an_RefPk()
		{
			var parameters = new Fakes.DataParameterCollection();
			var dbCommandMock = new Mock<IDbCommand> { DefaultValue = DefaultValue.Mock };
			dbCommandMock.SetupGet(command => command.Parameters).Returns(parameters);
			dbCommandMock.Setup(command => command.CreateParameter()).Returns(() => Mock.Of<IDbDataParameter>());
			var dbCommand = dbCommandMock.Object;
			var lookupRef = new ModelReferenced{ Id = 2004};

			var sql = SqlExtension.ToWhereSql((Expression<Func<ModelOne, bool>>) (model => model.Reference == lookupRef),
				dbCommand);
			
			Assert.AreEqual("reference = @reference", sql);
			Assert.AreEqual(lookupRef.Id, ((IDataParameter)parameters["@reference"]).Value);
		}
	}
}