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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DbModelFramework
{
	class EnumerableContract : ExecutionContract
	{
		internal static readonly DbRequirements DbRequirements = DbRequirements.Instance;

		#region sql
		
		private readonly PropertyInfo enumerableProperty;
		private readonly ModelProperty virtualEnumerableProperty;
		private readonly ModelProperty virtualModelProperty;
		internal readonly string tableName;
		private readonly string onCreateSql;
		private readonly string onInsertSql;
		private readonly string onDeleteSql;

		#endregion

		public EnumerableContract(Type modelType, PropertyInfo enumerableProperty)
		{
			Type enumerableItemType = enumerableProperty.PropertyType.GenericTypeArguments[0];

			tableName = $"{enumerableItemType.Name.ToLower()}sTo{modelType.Name.ToLower()}s";
			this.enumerableProperty = enumerableProperty;

			virtualEnumerableProperty = new ModelProperty(new VirtualPropertyInfo(enumerableItemType.Name.ToLower(), enumerableItemType));
			virtualModelProperty = new ModelProperty(new VirtualPropertyInfo(modelType.Name.ToLower(), modelType));

			var virtualModelProperties = new[] { virtualEnumerableProperty, virtualModelProperty };
			onCreateSql = DbRequirements.SqlEngine.CreateTable(tableName, virtualModelProperties);
			onInsertSql = DbRequirements.SqlEngine.InsertModel(tableName, virtualModelProperties);
			onDeleteSql = DbRequirements.SqlEngine.DeleteModel(tableName, virtualModelProperty);
		}

		public override void OnCreate(IDbConnection connection)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = onCreateSql;
				command.ExecuteNonQuery();
			}
		}

		public override void OnDelete<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = onDeleteSql;
				command.AddParameter($"@{virtualModelProperty.AttributeName}", virtualEnumerableProperty.Type, model.Id);
				command.ExecuteNonQuery();
			}
		}

		public override void OnInsert<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = onInsertSql;

				foreach(var item in (IEnumerable)enumerableProperty.GetValue(model))
				{
					command.Parameters.Clear();
					command.AddParameter($"@{virtualEnumerableProperty.AttributeName}", virtualEnumerableProperty.Type, item);
					command.AddParameter($"@{virtualModelProperty.AttributeName}", virtualEnumerableProperty.Type, model.Id);
					command.ExecuteNonQuery();
				}
			}
		}

		public override void OnUpdate()
		{
			throw new NotImplementedException();
		}

		public override void OnSelect()
		{
			throw new NotImplementedException();
		}
	}
}
