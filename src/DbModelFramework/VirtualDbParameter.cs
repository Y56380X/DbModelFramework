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

namespace DbModelFramework
{
	internal class VirtualDbParameter : IDbDataParameter
	{
		public DbType DbType { get; set; }
		public ParameterDirection Direction { get; set; }
		public bool IsNullable { get; }
		public string? ParameterName { get; set; }
		public string? SourceColumn { get; set; }
		public DataRowVersion SourceVersion { get; set; }
		public object? Value { get; set; }
		public byte Precision { get; set; }
		public byte Scale { get; set; }
		public int Size { get; set; }
	}
}