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
using System.Globalization;
using System.Reflection;

namespace DbModelFramework
{
	internal class VirtualPropertyInfo : PropertyInfo
	{
		#region properties

		public override PropertyAttributes Attributes => throw new NotImplementedException();

		public override bool CanRead => throw new NotImplementedException();

		public override bool CanWrite => throw new NotImplementedException();

		private readonly Type propertyType;
		public override Type PropertyType => propertyType;

		public override Type DeclaringType => throw new NotImplementedException();

		private readonly string name;
		public override string Name => name;

		public override Type ReflectedType => throw new NotImplementedException();

		#endregion

		public VirtualPropertyInfo(string name, Type type)
		{
			this.name = name;
			this.propertyType = type;
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			return null;
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			return new ParameterInfo[0];
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			return null;
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return false;
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
