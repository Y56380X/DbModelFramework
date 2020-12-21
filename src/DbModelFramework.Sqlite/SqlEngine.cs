/*
	Copyright (c) 2018-2020 Y56380X

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

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DbModelFramework.Sqlite
{
	internal class SqlEngine : DbModelFramework.SqlEngine
	{
		private static readonly Dictionary<DbType, string> DbTypeToStringDictionary = new Dictionary<DbType, string>
		{
			{ DbType.String, "TEXT" },
			{ DbType.Int32, "INTEGER" },
			{ DbType.Int16, "INTEGER" },
			{ DbType.Int64, "INTEGER" },
			{ DbType.Binary, "BLOB" },
			{ DbType.Single, "REAL" },
			{ DbType.Double, "REAL" },
			{ DbType.Decimal, "REAL" },
			{ DbType.DateTime, "TEXT" }
		};

		public override string CheckTable(string tableName) => $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";

		public override string CreateTable(string tableName, IEnumerable<ModelProperty> modelProperties)
		{
			static string BuildModelAttribute(ModelProperty prop) =>
				$"{prop.AttributeName} {DbTypeToString(prop.Type)}{(prop.IsPrimaryKey ? " PRIMARY KEY AUTOINCREMENT" : null)}{(prop.IsUnique ? " UNIQUE" : null)}";

			static string BuildForeignKeyAttribute(ModelProperty prop) =>
				$"FOREIGN KEY({prop.AttributeName}) REFERENCES {prop.ForeignKeyTableName}({prop.ForeignKeyReference!.AttributeName})";
			
			var modelAttributes = modelProperties.Select(BuildModelAttribute);
			var foreignKeyAttributes = modelProperties
				.Where(prop => prop.IsForeignKey)
				.Select(BuildForeignKeyAttribute);

			return $"CREATE TABLE {tableName} ({modelAttributes.ToChain()}{(foreignKeyAttributes.Any() ? $", {foreignKeyAttributes.ToChain()}" : null)});";
		}

		public override string GetLastPrimaryKey() => "SELECT last_insert_rowid();";

		internal static string DbTypeToString(DbType dbType) => DbTypeToStringDictionary[dbType];
	}
}
