/*
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
*/

using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DbModelFramework.Test
{
	[TestClass]
	public class FilterTest
	{
		#region test assets
		
		private class FilterTestModel : Model<FilterTestModel>
		{
			public string MyAttribute { get; set; }
		}

		#endregion
		
		[TestMethod]
		public void PrepareDbCommand_SqlIsCorrect()
		{
			// Arrange
			var command = Mock.Of<IDbCommand>(c => 
				c.CreateParameter() == Mock.Of<IDbDataParameter>()
				&& c.Parameters == Mock.Of<IDataParameterCollection>());

			// Act
			var filter = new Filter<FilterTestModel>(ftm => ftm.MyAttribute == "Test");
			filter.PrepareCommand(command, "{{0}}");

			// Assert
			Assert.AreEqual("myattribute = @myattribute", command.CommandText);
		}
		
		[TestMethod]
		public void PrepareDbCommand_ParameterValueIsSet()
		{
			// Arrange
			var parameters = new VirtualDataParameterCollection();
			var command = Mock.Of<IDbCommand>(c => 
				c.CreateParameter() == Mock.Of<IDbDataParameter>()
				&& c.Parameters == parameters);

			// Act
			var filter = new Filter<FilterTestModel>(ftm => ftm.MyAttribute == "Test");
			filter.PrepareCommand(command, "{{0}}");

			// Assert
			Assert.IsTrue(parameters.Contains("@myattribute"));
			Assert.AreEqual("Test", parameters["@myattribute"].Value);
			Assert.AreEqual(DbType.String, parameters["@myattribute"].DbType);
		}
	}
}