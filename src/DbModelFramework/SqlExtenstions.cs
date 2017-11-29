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
using System.Text;

namespace DbModelFramework
{
	static class SqlExtenstions
	{
		static readonly Dictionary<Type, DbType> DbTypeDictionary = new Dictionary<Type, DbType>
		{
			{ typeof(string), DbType.String }
		};

		public static string ToTableCreationSql(this IEnumerable<PropertyInfo> modelProperties)
		{
			StringBuilder stringBuilder = new StringBuilder();

			bool first = true;
			foreach (var property in modelProperties)
			{
				if (first)
				{
					stringBuilder.Append($"{property.Name.ToLower()} {property.PropertyType.ToDbType()}");
					first = false;
				}
				else
				{
					stringBuilder.Append($", {property.Name.ToLower()} {property.PropertyType.ToDbType()}");
				}
			}

			return stringBuilder.ToString();
		}

		public static DbType ToDbType(this Type type)
		{
			return DbTypeDictionary[type];
		}
	}
}
