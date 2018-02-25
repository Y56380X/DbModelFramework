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
using System.Collections.Generic;

namespace DbModelFramework
{
	class EnumerableContract : ExecutionContract
	{
		internal static readonly DbRequirements DbRequirements = DbRequirements.Instance;

		#region sql

		internal readonly IEnumerable<ModelProperty> virtualModelProperties;
		internal readonly string tableName;
		internal readonly string onCreateSql;

		#endregion

		public EnumerableContract(Type modelType, Type enumItemType)
		{
			tableName = $"{enumItemType.Name.ToLower()}sTo{modelType.Name.ToLower()}s";
			virtualModelProperties = new[] {
				new ModelProperty(new VirtualPropertyInfo(enumItemType.Name.ToLower(), enumItemType)),
				new ModelProperty(new VirtualPropertyInfo(modelType.Name.ToLower(), typeof(int)/*TODO: Type of models pk*/))
			};

			onCreateSql = DbRequirements.SqlEngine.CreateTable(tableName, virtualModelProperties);
		}

		public override void OnCreate()
		{
			throw new NotImplementedException();
		}

		public override void OnDelete()
		{
			throw new NotImplementedException();
		}

		public override void OnInsert()
		{
			throw new NotImplementedException();
		}

		public override void OnUpdate()
		{
			throw new NotImplementedException();
		}
	}
}
