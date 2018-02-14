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
using System.Collections.Generic;

namespace DbModelFramework.MySql
{
	class SqlEngine : DbModelFramework.SqlEngine
	{
		public override string CheckTable(string tableName)
		{
			throw new NotImplementedException();
		}

		public override string CreateTable(string tableName, IEnumerable<ModelProperty> modelProperties)
		{
			throw new NotImplementedException();
		}

		public override string GetLastPrimaryKey()
		{
			throw new NotImplementedException();
		}

		#region Old

		//static readonly Dictionary<DbType, string> DbTypeToStringDictionary = new Dictionary<DbType, string>
		//{
		//	{ DbType.String, "TEXT" },
		//	{ DbType.Int32, "INTEGER" },
		//	{ DbType.Int16, "INTEGER" },
		//	{ DbType.Int64, "INTEGER" },
		//	{ DbType.Binary, "BLOB" }
		//};

		//public override string GetCheckTableSql(string tableName)
		//{
		//	return $"SELECT table_name FROM information_schema.tables WHERE table_name='{tableName}' AND table_schema=DATABASE();";
		//}

		//public override string GetTableCreationSql(IEnumerable<ModelProperty> modelProperties)
		//{
		//	StringBuilder stringBuilder = new StringBuilder();

		//	bool first = true;
		//	foreach (var property in modelProperties)
		//	{
		//		if (first)
		//		{
		//			stringBuilder.Append($"{property.AttributeName} {DbTypeToString(property.Type)}");
		//			first = false;
		//		}
		//		else
		//		{
		//			stringBuilder.Append($", {property.AttributeName} {DbTypeToString(property.Type)}");
		//		}
		//	}

		//	// Build primary key constraints
		//	foreach (var property in modelProperties.Where(mp => mp.IsPrimaryKey))
		//		stringBuilder.Append($", PRIMARY KEY ({property.AttributeName})");

		//	// Build unique value constraints
		//	foreach (var property in modelProperties.Where(mp => mp.IsUnique))
		//		stringBuilder.Append($", UNIQUE ({property.AttributeName})");

		//	// Build foreign key constraints
		//	foreach (var property in modelProperties.Where(mp => mp.IsForeignKey))
		//		stringBuilder.Append($", FOREIGN KEY({property.AttributeName}) REFERENCES {property.ForeignKeyTableName}({property.ForeignKeyReference.AttributeName})");

		//	return stringBuilder.ToString();
		//}

		//private static string DbTypeToString(DbType dbType)
		//{
		//	return DbTypeToStringDictionary[dbType];
		//}

		#endregion
	}
}