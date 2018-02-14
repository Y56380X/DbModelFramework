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

namespace DbModelFramework.MySql
{
	class SqlEngine : DbModelFramework.SqlEngine
	{
		static readonly Dictionary<DbType, string> DbTypeToStringDictionary = new Dictionary<DbType, string>
		{
			{ DbType.String, "TEXT" },
			{ DbType.Int32, "INTEGER" },
			{ DbType.Int16, "INTEGER" },
			{ DbType.Int64, "INTEGER" },
			{ DbType.Binary, "BLOB" },
			{ DbType.Boolean, "BOOLEAN" }
		};

		static readonly Dictionary<DbType, string> DbTypeAsKeyToStringDictionary = new Dictionary<DbType, string>
		{
			{ DbType.String, "VARCHAR(255)" },
			{ DbType.Int32, "INTEGER" },
			{ DbType.Int16, "INTEGER" },
			{ DbType.Int64, "INTEGER" },
			{ DbType.Boolean, "BOOLEAN" }
		};

		public override string CheckTable(string tableName)
		{
			return $"SELECT table_name FROM information_schema.tables WHERE table_name='{tableName}' AND table_schema=DATABASE();";
		}

		public override string CreateTable(string tableName, IEnumerable<ModelProperty> modelProperties)
		{
			var modelAttributes = modelProperties.Select(prop =>
			{
				return $"{prop.AttributeName} {DbTypeToString(prop.Type, prop.IsUnique || prop.IsForeignKey || prop.IsPrimaryKey)}"
				+ $"{(prop.IsPrimaryKey ? " NOT NULL PRIMARY KEY AUTO_INCREMENT" : null)}{(prop.IsUnique ? " UNIQUE" : null)}";
			});

			return $"CREATE TABLE {tableName} ({modelAttributes.ToChain()});";
		}

		public override string GetLastPrimaryKey()
		{
			return "SELECT LAST_INSERT_ID();";
		}

		private static string DbTypeToString(DbType dbType, bool asKey = false)
		{
			return asKey ? DbTypeAsKeyToStringDictionary[dbType] : DbTypeToStringDictionary[dbType];
		}

		#region Old

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

		#endregion
	}
}
