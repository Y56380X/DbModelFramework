﻿/*
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
using System.Collections.Generic;
using System.Data;

namespace DbModelFramework
{
	internal abstract class ExecutionContract
	{
		public abstract void OnCreate(IDbConnection connection);

		public abstract void OnInsert<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model) 
			where TType : Model<TType, TPrimaryKey>, new() where TPrimaryKey : IComparable;

		public abstract void OnUpdate<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model) 
			where TType : Model<TType, TPrimaryKey>, new() where TPrimaryKey : IComparable;

		public abstract void OnDelete<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model) 
			where TType : Model<TType, TPrimaryKey>, new() where TPrimaryKey : IComparable;

		public abstract void OnSelect<TType, TPrimaryKey>(IDbConnection connection, Model<TType, TPrimaryKey> model) 
			where TType : Model<TType, TPrimaryKey>, new() where TPrimaryKey : IComparable;
	}

	internal static class ExecutionContractExtensions
	{
		public static void Execute(this IEnumerable<ExecutionContract> executionContracts, Action<ExecutionContract> action)
		{
			foreach (var executionContract in executionContracts)
				action.Invoke(executionContract);
		}
	}
}
