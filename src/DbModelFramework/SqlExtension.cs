/*
	Copyright (c) 2017-2020 Y56380X

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
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace DbModelFramework
{
	internal static class SqlExtension
	{
		private static readonly Dictionary<Type, DbType> TypeToDbTypeDictionary = new Dictionary<Type, DbType>
		{
			{ typeof(string), DbType.String },
			{ typeof(int), DbType.Int32 },
			{ typeof(short), DbType.Int16 },
			{ typeof(long), DbType.Int64 },
			{ typeof(byte[]), DbType.Binary },
			{ typeof(bool), DbType.Boolean },
			{ typeof(float), DbType.Single },
			{ typeof(double), DbType.Double },
			{ typeof(decimal), DbType.Decimal },
			{ typeof(DateTime), DbType.DateTime }
		};

		public static string ToWhereSql(this Expression selector, IDbCommand dbCommand)
		{
			switch (selector.NodeType)
			{
				case ExpressionType.Lambda when selector is LambdaExpression lambda:
					return lambda.Body.ToWhereSql(dbCommand);
				case ExpressionType.AndAlso when selector is BinaryExpression andAlso:
					return $"{andAlso.Left.ToWhereSql(dbCommand)} AND {andAlso.Right.ToWhereSql(dbCommand)}";
				case ExpressionType.OrElse when selector is BinaryExpression orElse:
					return $"{orElse.Left.ToWhereSql(dbCommand)} OR {orElse.Right.ToWhereSql(dbCommand)}";
				case ExpressionType.Equal when selector is BinaryExpression equal:
				{
					var value = GetValue(equal.Right);
					var paName = equal.Left.ToWhereSql(dbCommand);

					dbCommand.AddParameter($"@{paName}", ToDbType(value.GetType()), value);

					return $"{paName} = @{paName}";
				}
				case ExpressionType.NotEqual when selector is BinaryExpression notEqual:
				{
					var value = GetValue(notEqual.Right);
					var paName = notEqual.Left.ToWhereSql(dbCommand);

					dbCommand.AddParameter($"@{paName}", ToDbType(value.GetType()), value);

					return $"{paName} <> @{paName}";
				}
				case ExpressionType.MemberAccess when selector is MemberExpression memberAccess:
				{
					if (memberAccess.Expression is ParameterExpression)
						return memberAccess.Member.Name.ToLower();

					if (memberAccess.Member is FieldInfo fieldInfo && memberAccess.Expression is ConstantExpression cExpression)
						return fieldInfo.GetValue(cExpression.Value).ToString();

					return string.Empty;
				}
				case ExpressionType.Constant when selector is ConstantExpression constant:
					return constant.ToString();
				case ExpressionType.Call when selector is MethodCallExpression methodCall 
				                              && methodCall.Method.Name == nameof(string.StartsWith):
				{
					var value = GetValue(methodCall);
					var paName = methodCall.Object.ToWhereSql(dbCommand);

					dbCommand.AddParameter($"@{paName}", ToDbType(value.GetType()), value);

					return $"{paName} LIKE '{DbRequirements.Instance.SqlEngine.Wildcard}' + @{paName}";
				}
				case ExpressionType.Call when selector is MethodCallExpression methodCall 
				                              && methodCall.Method.Name == nameof(string.EndsWith):
				{
					var value = GetValue(methodCall);
					var paName = methodCall.Object.ToWhereSql(dbCommand);

					dbCommand.AddParameter($"@{paName}", ToDbType(value.GetType()), value);

					return $"{paName} LIKE @{paName} + '{DbRequirements.Instance.SqlEngine.Wildcard}'";
				}
				case ExpressionType.Call when selector is MethodCallExpression methodCall 
				                              && methodCall.Method.Name == nameof(string.Contains):
				{
					var value = GetValue(methodCall);
					var paName = methodCall.Object.ToWhereSql(dbCommand);

					dbCommand.AddParameter($"@{paName}", ToDbType(value.GetType()), value);

					return $"{paName} LIKE '{DbRequirements.Instance.SqlEngine.Wildcard}' + @{paName} + '{DbRequirements.Instance.SqlEngine.Wildcard}'";
				}
				default:
					return string.Empty;
			}
		}

		private static object GetValue(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Constant when expression is ConstantExpression constant:
					return constant.Value;
				case ExpressionType.MemberAccess when expression is MemberExpression memberAccess:
				{
					if (memberAccess.Expression is ParameterExpression)
						return memberAccess.Member.Name.ToLower();

					if (memberAccess.Member is FieldInfo fieldInfo &&
					    memberAccess.Expression is ConstantExpression cExpression)
					{
						var value = fieldInfo.GetValue(cExpression.Value);
						return value is IModel model
							? model.GetId()
							: value;
					}

					if (memberAccess.Member is PropertyInfo propertyInfo && memberAccess.Expression is MemberExpression submExpression)
					{
						if (submExpression.Expression is ConstantExpression subcExpression)
						{
							var fieldInfoValue = ((FieldInfo)submExpression.Member).GetValue(subcExpression.Value);
							return propertyInfo.GetValue(fieldInfoValue, null);
						}
						else
						{
							var subPropertyValue = GetValue(submExpression.Expression);
							var propertyParentObject = ((PropertyInfo)submExpression.Member).GetValue(subPropertyValue);

							return propertyInfo.GetValue(propertyParentObject);
						}
					}

					return string.Empty;
				}
				case ExpressionType.Call when expression is MethodCallExpression methodCall:
					return GetValue(methodCall.Arguments[0]);
				default:
					return DBNull.Value;
			}
		}

		public static DbType ToDbType(this Type type)
		{
			if (type.TryGetGenericBaseClass(typeof(Model<,>), out var genericBase))
			{
				return genericBase!
					.GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic)
					.PropertyType.ToDbType();
			}

			return type.IsEnum ? type.GetEnumUnderlyingType().ToDbType() : TypeToDbTypeDictionary[type];
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
