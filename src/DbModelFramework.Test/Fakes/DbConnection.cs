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
		private static Dictionary<string, int> customExecuteNonQueryResults = new Dictionary<string, int>();
		private static Dictionary<string, IDataReader> customExecuteReaderResults = new Dictionary<string, IDataReader>();
		private static Dictionary<string, object> customExecuteScalarResults = new Dictionary<string, object>();

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
			return Mock.Of<IDbTransaction>();
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			var dbTransactionMock = new Mock<IDbTransaction>(il) { DefaultValue = DefaultValue.Mock };
			dbTransactionMock.SetupAllProperties();

			return dbTransactionMock.Object;
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
			var commandMock = new Mock<IDbCommand>() { DefaultValue = DefaultValue.Mock };
			commandMock.SetupAllProperties();

			// Setup execusion
			commandMock.Setup(c => c.ExecuteNonQuery()).Returns(() => 
			{
					if (customExecuteNonQueryResults.ContainsKey(commandMock.Object.CommandText))
						return customExecuteNonQueryResults[commandMock.Object.CommandText];
					else
						return default(int);
			});
			commandMock.Setup(c => c.ExecuteReader()).Returns(() =>
			{
				if (customExecuteReaderResults.ContainsKey(commandMock.Object.CommandText))
					return customExecuteReaderResults[commandMock.Object.CommandText];
				else
					return Mock.Of<IDataReader>();
			});
			commandMock.Setup(c => c.ExecuteScalar()).Returns(() =>
			{
				if (customExecuteScalarResults.ContainsKey(commandMock.Object.CommandText))
					return customExecuteScalarResults[commandMock.Object.CommandText];
				else
					return default(object);
			});

			// Setup parameters
			commandMock.Setup(c => c.CreateParameter()).Returns(Mock.Of<IDbDataParameter>);

			var parameters = new DataParameterCollection();
			commandMock.SetupGet(c => c.Parameters).Returns(parameters);

			var command = commandMock.Object;
			CreatedCommands.Add(command);

			return command;
		}

		public void Dispose() { }

		public void Open()
		{
			throw new System.NotImplementedException();
		}

		public static void AddCustomExecuteNonQueryResult(string sql, int result)
		{
			customExecuteNonQueryResults.Add(sql, result);
		}

		public static void AddCustomExecuteScalarResult(string sql, object result)
		{
			customExecuteScalarResults.Add(sql, result);
		}

		public static void AddCustomExecuteReaderResult(string sql, IDataReader result)
		{
			customExecuteReaderResults.Add(sql, result);
		}

		public static void ClearCustomExecuteResults()
		{
			customExecuteNonQueryResults.Clear();
			customExecuteReaderResults.Clear();
			customExecuteScalarResults.Clear();
		}
	}
}
