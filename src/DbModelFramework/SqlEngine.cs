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
using System.Linq;

namespace DbModelFramework
{
	public abstract class SqlEngine
	{
		public abstract string CreateTable(string tableName, IEnumerable<ModelProperty> modelProperties);
		public abstract string CheckTable(string tableName);
		public abstract string GetLastPrimaryKey();
		
		public virtual string InsertModel(string tableName, IEnumerable<ModelProperty> modelProperties)
		{
			var modelAttributes = modelProperties
				.Where(prop => !prop.IsPrimaryKey)
				.Select(prop => prop.AttributeName)
				.ToArray();

			if (!modelAttributes.Any())
				return $"INSERT INTO {tableName} DEFAULT VALUES;";

			var modelParameters = modelProperties
				.Where(prop => !prop.IsPrimaryKey)
				.Select(prop => $"@{prop.AttributeName}");

			return $"INSERT INTO {tableName} ({modelAttributes.ToChain()}) VALUES ({modelParameters.ToChain()});";
		}

		public virtual string UpdateModel(string tableName, IEnumerable<ModelProperty> modelProperties)
		{
			var primaryKeyProperty = modelProperties.Single(prop => prop.IsPrimaryKey);
			var setParameters = modelProperties
				.Where(prop => !prop.IsPrimaryKey)
				.Select(prop => $"{prop.AttributeName} = @{prop.AttributeName}");

			return $"UPDATE {tableName} SET {setParameters.ToChain()} WHERE {primaryKeyProperty.AttributeName} = @{primaryKeyProperty.AttributeName};";
		}

		public virtual string DeleteModel(string tableName, ModelProperty primaryKeyProperty)
		{
			return $"DELETE FROM {tableName} WHERE {primaryKeyProperty.AttributeName} = @{primaryKeyProperty.AttributeName};";
		}

		public virtual string SelectModelAllEntries(string tableName, IEnumerable<ModelProperty> modelProperties)
		{
			var modelAttributes = modelProperties.Select(prop => prop.AttributeName);

			return $"SELECT {modelAttributes.ToChain()} FROM {tableName};";
		}

		public virtual string SelectModelEntryByPrimaryKey(string tableName, IEnumerable<ModelProperty> modelProperties)
		{
			var primaryKeyProperty = modelProperties.Single(prop => prop.IsPrimaryKey);
			var modelAttributes = modelProperties.Select(prop => prop.AttributeName);

			return $"SELECT {modelAttributes.ToChain()} FROM {tableName} WHERE {primaryKeyProperty.AttributeName} = @{primaryKeyProperty.AttributeName};";
		}

		public virtual string SelectModelEntriesByCustomCondition(string tableName, IEnumerable<ModelProperty> modelProperties, string placeholder)
		{
			var modelAttributes = modelProperties.Select(prop => prop.AttributeName);

			return $"SELECT {modelAttributes.ToChain()} FROM {tableName} WHERE {placeholder};";
		}

		public virtual string SelectModelEntriesByForeignKey(string tableName, IEnumerable<ModelProperty> modelProperties, ModelProperty modelForeignKeyProperty)
		{
			var modelAttributes = modelProperties.Select(prop => prop.AttributeName);

			return $"SELECT {modelAttributes.ToChain()} FROM {tableName} WHERE {modelForeignKeyProperty.AttributeName} = @{modelForeignKeyProperty.AttributeName};";
		}
	}
}
