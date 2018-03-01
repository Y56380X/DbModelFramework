/**
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

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DbModelFramework.Sqlite
{
	class SqlEngine : DbModelFramework.SqlEngine
	{
		static readonly Dictionary<DbType, string> DbTypeToStringDictionary = new Dictionary<DbType, string>
		{
			{ DbType.String, "TEXT" },
			{ DbType.Int32, "INTEGER" },
			{ DbType.Int16, "INTEGER" },
			{ DbType.Int64, "INTEGER" },
			{ DbType.Binary, "BLOB" }
		};

		public override string CheckTable(string tableName)
		{
			return $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";
		}

		public override string CreateTable(string tableName, IEnumerable<ModelProperty> modelProperties)
		{
			var modelAttributes = modelProperties.Select(prop =>
			{
				return $"{prop.AttributeName} {DbTypeToString(prop.Type)}{(prop.IsPrimaryKey ? " PRIMARY KEY AUTOINCREMENT" : null)}{(prop.IsUnique ? " UNIQUE" : null)}";
			});

			var foreignKeyAttributes = modelProperties.Where(prop => prop.IsForeignKey).Select(prop =>
			{
				return $"FOREIGN KEY({prop.AttributeName}) REFERENCES {prop.ForeignKeyTableName}({prop.ForeignKeyReference.AttributeName})";
			});

			return $"CREATE TABLE {tableName} ({modelAttributes.ToChain()}{(foreignKeyAttributes.Count() > 0 ? $", {foreignKeyAttributes.ToChain()}" : null)});";
		}

		public override string GetLastPrimaryKey()
		{
			return "SELECT last_insert_rowid();";
		}

		public static string DbTypeToString(DbType dbType)
		{
			return DbTypeToStringDictionary[dbType];
		}
	}
}
