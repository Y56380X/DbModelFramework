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

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using static DbModelFramework.DependencyInjection;

namespace DbModelFramework
{
	public class Model<TType> where TType : new()
	{
		internal static readonly string TableName = $"{typeof(TType).Name.ToLower()}s";
		internal static readonly IEnumerable<PropertyInfo> ModelProperties = typeof(TType).GetProperties();

		internal static class Sql
		{
			public static readonly string CheckTable = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{TableName}';";

			static Sql()
			{
				if (!Check())
				{
					// Setup table
				}
			}

			private static bool Check()
			{
				bool result;

				using (var connection = InjectionContainer.GetExport<IDbConnection>())
				using (var command = connection.CreateCommand())
				{
					command.CommandText = CheckTable;
					result = command.ExecuteScalar() != null;
				}

				return result;
			}
		}

		protected Model()
		{
		}

		public static Model<TType> Get()
		{
			throw new NotImplementedException();
		}

		public static Model<TType> Create()
		{
			throw new NotImplementedException();
		}
	}
}
