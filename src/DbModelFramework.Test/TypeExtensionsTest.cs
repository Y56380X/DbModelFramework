﻿/**
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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbModelFramework.Test
{
	[TestClass]
	public class TypeExtensionsTest
	{
		[TestMethod]
		public void StringDefaultValue()
		{
			var defaultValue = typeof(string).GetDefault();

			Assert.AreEqual(default(string), defaultValue);
		}

		[TestMethod]
		public void Int32DefaultValue()
		{
			var defaultValue = typeof(int).GetDefault();

			Assert.AreEqual(default(int), defaultValue);
		}

		[TestMethod]
		public void Int16DefaultValue()
		{
			var defaultValue = typeof(short).GetDefault();

			Assert.AreEqual(default(short), defaultValue);
		}

		[TestMethod]
		public void Int64DefaultValue()
		{
			var defaultValue = typeof(long).GetDefault();

			Assert.AreEqual(default(long), defaultValue);
		}

		[TestMethod]
		public void DateTimeDefaultValue()
		{
			var defaultValue = typeof(System.DateTime).GetDefault();

			Assert.AreEqual(default(System.DateTime), defaultValue);
		}
	}
}