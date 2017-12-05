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
using System.Data;
using System.Reflection;

namespace DbModelFramework
{
	class ModelProperty
	{
		#region fields

		private PropertyInfo property;

		#endregion

		#region properties

		public string PropertyName { get; private set; }
		public string AttributeName { get; private set; }
		public DbType Type { get; private set; }
		public object DefaultValue { get; private set; }
		public bool IsPrimaryKey { get; private set; }
		public bool IsUnique { get; private set; }

		#endregion

		#region constructors

		public ModelProperty(PropertyInfo property)
		{
			PropertyName = property.Name;
			AttributeName = property.Name.ToLower();
			Type = property.PropertyType.ToDbType();
			DefaultValue = property.PropertyType.GetDefault();
			IsPrimaryKey = Attribute.IsDefined(property, typeof(PrimaryKeyAttribute));
			IsUnique = Attribute.IsDefined(property, typeof(DbUniqueAttribute));

			this.property = property;
		}

		#endregion

		#region methods

		public void SetValue(object model, object value)
		{
			if (value != null && !property.PropertyType.IsInstanceOfType(value))
				property.SetValue(model, Convert.ChangeType(value, property.PropertyType));
			else
				property.SetValue(model, value);
		}

		public object GetValue(object model)
		{
			return property.GetValue(model);
		}

		#endregion
	}
}
