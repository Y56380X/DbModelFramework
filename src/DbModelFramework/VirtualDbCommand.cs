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

using System;
using System.Data;

namespace DbModelFramework
{
	internal class VirtualDbCommand : IDbCommand
	{
		#region properties
		
		public string? CommandText { get; set; }
		public int CommandTimeout { get; set; }
		public CommandType CommandType { get; set; }
		public IDbConnection Connection
		{
			get => throw new NotSupportedException();
			set => throw new NotSupportedException();
		}
		public IDataParameterCollection Parameters { get; } = new VirtualDataParameterCollection();
		public IDbTransaction Transaction
		{
			set => throw new NotSupportedException();
			get => throw new NotSupportedException();
		}
		public UpdateRowSource UpdatedRowSource { get; set; }
		
		#endregion
		
		public void Dispose() => Parameters.Clear();

		public void Cancel() => throw new NotSupportedException();
		public IDbDataParameter CreateParameter() => new VirtualDbParameter();
		public int ExecuteNonQuery() => throw new NotSupportedException();
		public IDataReader ExecuteReader() => throw new NotSupportedException();
		public IDataReader ExecuteReader(CommandBehavior behavior) => throw new NotSupportedException();
		public object ExecuteScalar() => throw new NotSupportedException();
		public void Prepare() => throw new NotSupportedException();
	}
}