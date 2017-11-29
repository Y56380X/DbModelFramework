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

using Moq;
using System.Collections.Generic;
using System.Composition;
using System.Data;

namespace DbModelFramework.Test.Fakes
{
	[Export(typeof(IDbConnection))]
	class DbConnection : IDbConnection
	{
		#region properties

		public static List<IDbCommand> CreatedCommands { get; } = new List<IDbCommand>();

		#region IDbConnection

		public string ConnectionString { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public int ConnectionTimeout => throw new System.NotImplementedException();

		public string Database => throw new System.NotImplementedException();

		public ConnectionState State => throw new System.NotImplementedException();

		#endregion

		#endregion

		public IDbTransaction BeginTransaction()
		{
			throw new System.NotImplementedException();
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			throw new System.NotImplementedException();
		}

		public void ChangeDatabase(string databaseName)
		{
			throw new System.NotImplementedException();
		}

		public void Close()
		{
			throw new System.NotImplementedException();
		}

		public IDbCommand CreateCommand()
		{
			var command = Mock.Of<IDbCommand>();
			CreatedCommands.Add(command);

			return command;
		}

		public void Dispose() { }

		public void Open()
		{
			throw new System.NotImplementedException();
		}
	}
}
