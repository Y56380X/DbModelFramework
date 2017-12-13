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
using System.Linq;
using System.Linq.Expressions;
using static DbModelFramework.DependencyInjection;

namespace DbModelFramework
{
	public class Model<TType> : Model<TType, long> where TType : new()
	{

	}

	public class Model<TType, TPrimaryKey> where TType : new() where TPrimaryKey : IComparable
	{
		#region fields

		internal static readonly string TableName = $"{typeof(TType).Name.ToLower()}s";
		internal static readonly IEnumerable<ModelProperty> ModelProperties = typeof(TType).GetModelProperties();
		internal static readonly ModelProperty PrimaryKeyProperty = ModelProperties.Single(prop => prop.IsPrimaryKey);

		#endregion

		internal static class Sql
		{
			public static readonly string CheckTable = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{TableName}';";
			public static readonly string CreateTable = $"CREATE TABLE {TableName} ({ModelProperties.ToTableCreationSql()});";
			public static readonly string Insert = $"INSERT INTO {TableName} ({ModelProperties.ToAttributeChainSql()}) VALUES ({ModelProperties.ToInsertParameterChainSql()});";
			public static readonly string LastPrimaryKey = "SELECT last_insert_rowid();";
			public static readonly string SelectAll = $"SELECT {ModelProperties.ToAttributeChainSql(true)} FROM {TableName};";
			public static readonly string SelectByPrimaryKey = $"SELECT {ModelProperties.ToAttributeChainSql(true)} FROM {TableName} WHERE {PrimaryKeyProperty.AttributeName} = @{PrimaryKeyProperty.AttributeName};";
			public static readonly string SelectByCustomCondition = $"SELECT {ModelProperties.ToAttributeChainSql(true)} FROM {TableName} WHERE {{0}};";
			public static readonly string Delete = $"DELETE FROM {TableName} WHERE {PrimaryKeyProperty.AttributeName} = @{PrimaryKeyProperty.AttributeName};";
			public static readonly string Update = $"UPDATE {TableName} SET {ModelProperties.ToUpdateSql()} WHERE {PrimaryKeyProperty.AttributeName} = @{PrimaryKeyProperty.AttributeName};";

			static Sql()
			{
				if (!Check())
					Create();
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

			private static void Create()
			{
				using (var connection = InjectionContainer.GetExport<IDbConnection>())
				using (var command = connection.CreateCommand())
				{
					command.CommandText = CreateTable;
					command.ExecuteNonQuery();
				}
			}
		}

		#region properties

		[PrimaryKey]
		protected TPrimaryKey Id { get; set; }

		#endregion

		#region methods

		protected Model()
		{
		}

		public bool Save()
		{
			int changed;

			using (var connection = InjectionContainer.GetExport<IDbConnection>())
			using (var command = connection.CreateCommand())
			{
				command.CommandText = Sql.Update;
				command.AddParameter($"@{PrimaryKeyProperty.AttributeName}", PrimaryKeyProperty.Type, PrimaryKeyProperty.GetValue(this));

				foreach (var property in ModelProperties.Where(prop => !prop.IsPrimaryKey))
					command.AddParameter($"@{property.AttributeName}", property.Type, property.GetValue(this) ?? DBNull.Value);

				changed = command.ExecuteNonQuery();
			}

			return changed == 1;
		}

		public bool Delete()
		{
			int changed;

			using (var connection = InjectionContainer.GetExport<IDbConnection>())
			using (var command = connection.CreateCommand())
			{
				command.CommandText = Sql.Delete;
				command.AddParameter($"@{PrimaryKeyProperty.AttributeName}", PrimaryKeyProperty.Type, PrimaryKeyProperty.GetValue(this));

				changed = command.ExecuteNonQuery();
			}

			return changed == 1;
		}

		public static TType Get(TPrimaryKey primaryKey)
		{
			TType model = default(TType);

			using (var connection = InjectionContainer.GetExport<IDbConnection>())
			using (var command = connection.CreateCommand())
			{
				command.CommandText = Sql.SelectByPrimaryKey;
				command.AddParameter($"@{PrimaryKeyProperty.AttributeName}", PrimaryKeyProperty.Type, primaryKey);

				using (var dataReader = command.ExecuteReader())
				{
					if (dataReader.Read())
						model = InstanciateModel(dataReader);
				}
			}

			return model;
		}

		public static IEnumerable<TType> Get()
		{
			var models = new List<TType>();

			using (var connection = InjectionContainer.GetExport<IDbConnection>())
			using (var command = connection.CreateCommand())
			{
				command.CommandText = Sql.SelectAll;

				using (var dataReader = command.ExecuteReader())
				{
					while (dataReader.Read())
						models.Add(InstanciateModel(dataReader));
				}
			}

			return models;
		}

		public static IEnumerable<TType> Get(Expression<Func<TType, bool>> selector)
		{
			var models = new List<TType>();

			using (var connection = InjectionContainer.GetExport<IDbConnection>())
			using (var command = connection.CreateCommand())
			{
				command.CommandText = Sql.SelectByCustomCondition.Replace("{0}", selector.ToWhereSql(command));

				using (var dataReader = command.ExecuteReader())
				{
					while (dataReader.Read())
						models.Add(InstanciateModel(dataReader));
				}
			}

			return models;
		}

		public static TType Create()
		{
			var model = new TType();

			using (var connection = InjectionContainer.GetExport<IDbConnection>())
			{
				// Insert
				using (var command = connection.CreateCommand())
				{
					command.CommandText = Sql.Insert;

					foreach (var property in ModelProperties)
						command.AddParameter($"@{property.AttributeName}", property.Type, property.GetValue(model) ?? DBNull.Value);

					command.ExecuteNonQuery();
				}

				// Update primary key
				using (var command = connection.CreateCommand())
				{
					command.CommandText = Sql.LastPrimaryKey;
					PrimaryKeyProperty.SetValue(model, command.ExecuteScalar());
				}
			}

			return model;
		}

		private static TType InstanciateModel(IDataReader dataReader)
		{
			var model = new TType();

			// Get value from db for each model property
			foreach (var property in ModelProperties)
			{
				if (dataReader[property.AttributeName.ToLower()] is DBNull)
					property.SetValue(model, property.DefaultValue);
				else
					property.SetValue(model, dataReader[property.AttributeName]);
			}

			return model;
		}

		#endregion
	}
}
