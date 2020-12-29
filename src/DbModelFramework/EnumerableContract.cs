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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DbModelFramework
{
	internal class EnumerableContract : ExecutionContract
	{
		internal static readonly DbRequirements DbRequirements = DbRequirements.Instance;

		#region sql
		
		private delegate object GetModelDelegate(object foreignKey);
		private delegate object GetForeignKeyDelegate(object model);

		private readonly PropertyInfo enumerableProperty;
		private readonly Type enumerableItemType;
		private readonly Type listType;
		private readonly GetModelDelegate getModel;
		private readonly GetForeignKeyDelegate getForeignKey;
		private readonly ModelProperty virtualEnumerableProperty;
		private readonly ModelProperty virtualModelProperty;
		internal readonly string tableName;
		private readonly string onCreateSql;
		private readonly string onInsertSql;
		private readonly string onDeleteSql;
		private readonly string onSelectSql;

		#endregion

		public EnumerableContract(Type modelType, PropertyInfo enumerableProperty)
		{
			enumerableItemType = enumerableProperty.PropertyType.GenericTypeArguments[0];
			listType = typeof(List<>).MakeGenericType(enumerableItemType);

			tableName = $"{enumerableItemType.Name.ToLower()}sTo{modelType.Name.ToLower()}s";
			this.enumerableProperty = enumerableProperty;

			virtualEnumerableProperty = new ModelProperty(new VirtualPropertyInfo(enumerableItemType.Name.ToLower(), enumerableItemType));
			virtualModelProperty = new ModelProperty(new VirtualPropertyInfo(modelType.Name.ToLower(), modelType));

			// Use different getModel and getForeignKey Methods on non Model enumerables
			if (enumerableItemType.TryGetGenericBaseClass(typeof(Model<,>), out var baseClass))
			{
				var idProperty = baseClass!.GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
				var getMethod = baseClass.GetMethod("Get", new[] { idProperty.PropertyType });

				getModel = foreignKey => getMethod.Invoke(null, new[] { foreignKey });
				getForeignKey = model => idProperty.GetValue(model);
			}
			else
			{
				getModel = Return;
				getForeignKey = Return;
			}

			// Setup sql statements
			var virtualModelProperties = new[] { virtualEnumerableProperty, virtualModelProperty };
			onCreateSql = DbRequirements.SqlEngine.CreateTable(tableName, virtualModelProperties);
			onInsertSql = DbRequirements.SqlEngine.InsertModel(tableName, virtualModelProperties);
			onDeleteSql = DbRequirements.SqlEngine.DeleteModel(tableName, virtualModelProperty);
			onSelectSql = DbRequirements.SqlEngine.SelectModelEntriesByForeignKey(tableName, virtualModelProperties, virtualModelProperty);
		}

		public override void OnCreate(IDbConnection connection)
		{
			using var command = connection.CreateCommand();
			
			command.CommandText = onCreateSql;
			command.ExecuteNonQuery();
		}

		public override void OnDelete<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model)
		{
			using var command = connection.CreateCommand();
			
			command.CommandText = onDeleteSql;
			command.AddParameter($"@{virtualModelProperty.AttributeName}", virtualEnumerableProperty.Type, model.Id);
			command.ExecuteNonQuery();
		}

		public override void OnInsert<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model)
		{
			using var command = connection.CreateCommand();
			
			command.CommandText = onInsertSql;

			foreach (var item in (IEnumerable)enumerableProperty.GetValue(model))
			{
				command.Parameters.Clear();
				command.AddParameter($"@{virtualEnumerableProperty.AttributeName}", virtualEnumerableProperty.Type, getForeignKey(item));
				command.AddParameter($"@{virtualModelProperty.AttributeName}", virtualModelProperty.Type, model.Id);
				command.ExecuteNonQuery();
			}
		}

		public override void OnUpdate<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model)
		{
			OnDelete(connection, model);
			OnInsert(connection, model);
		}

		public override void OnSelect<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model)
		{
			var enumerable = (IList)Activator.CreateInstance(listType);

			using var command = connection.CreateCommand();
			
			command.CommandText = onSelectSql;
			command.AddParameter($"@{virtualModelProperty.AttributeName}", virtualEnumerableProperty.Type, model.Id);

			using var dataReader = command.ExecuteReader();
			while (dataReader.Read())
			{
				var dataField = dataReader[virtualEnumerableProperty.AttributeName];
				enumerable.Add(getModel(dataField));
			}

			enumerableProperty.SetValue(model, enumerable);
		}

		private object Return(object source) => 
			enumerableItemType.IsInstanceOfType(source) 
				? source 
				: Convert.ChangeType(source, enumerableItemType);
	}
}
