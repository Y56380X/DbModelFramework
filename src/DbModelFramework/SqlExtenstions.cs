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
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DbModelFramework
{
	static class SqlExtenstions
	{
		static readonly Dictionary<Type, DbType> TypeToDbTypeDictionary = new Dictionary<Type, DbType>
		{
			{ typeof(string), DbType.String },
			{ typeof(int), DbType.Int32 },
			{ typeof(short), DbType.Int16 },
			{ typeof(long), DbType.Int64 },
			{ typeof(byte[]), DbType.Binary }
		};

		public static string ToAttributeChainSql(this IEnumerable<ModelProperty> modelProperties, bool withPrimaryKey = false)
		{
			StringBuilder stringBuilder = new StringBuilder();

			bool first = true;
			foreach (var property in modelProperties)
			{
				if (!withPrimaryKey && property.IsPrimaryKey)
					continue;

				if (first)
				{
					stringBuilder.Append($"{property.AttributeName}");
					first = false;
				}
				else
				{
					stringBuilder.Append($", {property.AttributeName}");
				}
			}

			return stringBuilder.ToString();
		}

		public static string ToInsertParameterChainSql(this IEnumerable<ModelProperty> modelProperties)
		{
			StringBuilder stringBuilder = new StringBuilder();

			bool first = true;
			foreach (var property in modelProperties)
			{
				if (property.IsPrimaryKey)
					continue;

				if (first)
				{
					stringBuilder.Append($"@{property.AttributeName}");
					first = false;
				}
				else
				{
					stringBuilder.Append($", @{property.AttributeName}");
				}
			}

			return stringBuilder.ToString();
		}

		public static string ToUpdateSql(this IEnumerable<ModelProperty> modelProperties)
		{
			StringBuilder stringBuilder = new StringBuilder();

			bool first = true;
			foreach (var property in modelProperties)
			{
				if (property.IsPrimaryKey)
					continue;

				if (first)
				{
					stringBuilder.Append($"{property.AttributeName} = @{property.AttributeName}");
					first = false;
				}
				else
				{
					stringBuilder.Append($", {property.AttributeName} = @{property.AttributeName}");
				}
			}

			return stringBuilder.ToString();
		}

		public static string ToWhereSql(this Expression selector, IDbCommand dbCommand)
		{
			switch (selector.NodeType)
			{
				case ExpressionType.Lambda:
					return (selector as LambdaExpression).Body.ToWhereSql(dbCommand);
				case ExpressionType.AndAlso:
					return $"{(selector as BinaryExpression).Left.ToWhereSql(dbCommand)} AND {(selector as BinaryExpression).Right.ToWhereSql(dbCommand)}";
				case ExpressionType.OrElse:
					return $"{(selector as BinaryExpression).Left.ToWhereSql(dbCommand)} OR {(selector as BinaryExpression).Right.ToWhereSql(dbCommand)}";
				case ExpressionType.Equal:
					{
						var bExpression = selector as BinaryExpression;
						var value = GetValue(bExpression.Right);
						var paName = bExpression.Left.ToWhereSql(dbCommand);

						dbCommand.AddParameter($"@{paName}", ToDbType(value.GetType()), value);

						return $"{paName} = @{paName}";
					}
				case ExpressionType.NotEqual:
					{
						var bExpression = selector as BinaryExpression;
						var value = GetValue(bExpression.Right);
						var paName = bExpression.Left.ToWhereSql(dbCommand);

						dbCommand.AddParameter($"@{paName}", ToDbType(value.GetType()), value);

						return $"{paName} <> @{paName}";
					}
				case ExpressionType.MemberAccess:
					{
						var mExpression = selector as MemberExpression;

						if (mExpression.Expression is ParameterExpression)
							return mExpression.Member.Name.ToLower();

						if (mExpression.Member is FieldInfo fieldInfo && mExpression.Expression is ConstantExpression cExpression)
							return fieldInfo.GetValue(cExpression.Value).ToString();

						return string.Empty;
					}
				case ExpressionType.Constant:
					return (selector as ConstantExpression).ToString();
				default:
					return string.Empty;
			}
		}

		private static object GetValue(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Constant:
					return (expression as ConstantExpression).Value;
				case ExpressionType.MemberAccess:
					{
						var mExpression = expression as MemberExpression;

						if (mExpression.Expression is ParameterExpression)
							return mExpression.Member.Name.ToLower();

						if (mExpression.Member is FieldInfo fieldInfo && mExpression.Expression is ConstantExpression cExpression)
							return fieldInfo.GetValue(cExpression.Value);

						if (mExpression.Member is PropertyInfo propertyInfo && mExpression.Expression is MemberExpression submExpression)
						{
							if (submExpression.Expression is ConstantExpression subcExpression)
							{
								var fieldInfoValue = (submExpression.Member as FieldInfo).GetValue(subcExpression.Value);
								return propertyInfo.GetValue(fieldInfoValue, null);
							}
							else
							{
								var subPropertyValue = GetValue(submExpression.Expression);
								var propertyParentObject = (submExpression.Member as PropertyInfo).GetValue(subPropertyValue);

								return propertyInfo.GetValue(propertyParentObject);
							}
						}

						return string.Empty;
					}
				default:
					return DBNull.Value;
			}
		}

		public static DbType ToDbType(this Type type)
		{
			return TypeToDbTypeDictionary[type];
		}

		public static void AddParameter(this IDbCommand command, string parameterName, DbType dbType, object value)
		{
			var parameter = command.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.DbType = dbType;
			parameter.Value = value;
			command.Parameters.Add(parameter);
		}
	}
}
