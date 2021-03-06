﻿/*
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DbModelFramework
{
	internal static class TypeExtensions
	{
		private static readonly Type[] EnumerableBlacklist =
		{
			typeof(string),
			typeof(byte[])
		};

		public static object? GetDefault(this Type type) => 
			type.IsValueType ? Activator.CreateInstance(type) : null;

		public static IEnumerable<ModelProperty> GetModelProperties(this Type modelType)
		{
			// Get all properties (without ignored ones and enumerables)
			var properties = modelType
				.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				.Where(prop => !Attribute.IsDefined(prop, typeof(DbIgnoreAttribute)) && (EnumerableBlacklist.Contains(prop.PropertyType) || !typeof(IEnumerable).IsAssignableFrom(prop.PropertyType)));
			
			return properties.Select(prop => new ModelProperty(prop));
		}

		public static bool TryGetGenericBaseClass(this Type propertyType, Type genericType, out Type? genericBaseClass)
		{
			propertyType = propertyType.BaseType;
			while (propertyType != typeof(object))
			{
				if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == genericType)
				{
					genericBaseClass = propertyType;
					return true;
				}

				propertyType = propertyType.BaseType;
			}

			genericBaseClass = default;
			return false;
		}

		public static IEnumerable<ExecutionContract> GetExecutionContracts(this Type modelType, ModelProperty modelPrimaryKey)
		{
			var executionContracts = new List<ExecutionContract>();

			// Setup execution contracts for enumerables
			var enumerables = modelType
				.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				.Where(prop => !Attribute.IsDefined(prop, typeof(DbIgnoreAttribute)) && !EnumerableBlacklist.Contains(prop.PropertyType) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType));

			executionContracts.AddRange(enumerables.Select(en => new EnumerableContract(modelType, en)));

			return executionContracts;
		}
	}
}
