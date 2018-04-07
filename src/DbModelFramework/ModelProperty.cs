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
	public class ModelProperty
	{
		#region delegates

		delegate object ConvertValue(object value, Type targetType);

		#endregion

		#region fields

		private PropertyInfo property;
		private MethodInfo foreignKeyLoader;
		private ConvertValue convertValue;

		#endregion

		#region properties

		public virtual string PropertyName { get; private set; }
		public virtual string AttributeName { get; private set; }
		public virtual DbType Type { get; private set; }
		public virtual object DefaultValue { get; private set; }
		public virtual bool IsPrimaryKey { get; private set; }
		public virtual bool IsUnique { get; private set; }
		public virtual bool IsForeignKey { get; private set; }
		public virtual ModelProperty ForeignKeyReference { get; private set; }
		public virtual string ForeignKeyTableName { get; private set; }

		#endregion

		#region constructors

		internal ModelProperty() { }

		public ModelProperty(PropertyInfo property)
		{
			PropertyName = property.Name;
			AttributeName = property.Name.ToLower();
			IsForeignKey = property.PropertyType.TryGetGenericBaseClass(typeof(Model<,>), out Type baseClass);
			ForeignKeyReference = IsForeignKey ? GetForeignKeyReference(baseClass) : null;
			ForeignKeyTableName = IsForeignKey ? GetForeignKeyTableName(baseClass) : null;
			Type = IsForeignKey ? ForeignKeyReference.Type : property.PropertyType.ToDbType();
			DefaultValue = property.PropertyType.GetDefault();
			IsPrimaryKey = Attribute.IsDefined(property, typeof(PrimaryKeyAttribute));
			IsUnique = Attribute.IsDefined(property, typeof(DbUniqueAttribute));

			foreignKeyLoader = IsForeignKey ? baseClass.GetMethod("Get", new Type[] { ForeignKeyReference.property.PropertyType }) : null;

			this.property = property;
			convertValue = property.PropertyType.IsEnum ? (ConvertValue)ToEnumValue : ChangeType;
		}

		#endregion

		#region methods

		public void SetValue(object model, object value)
		{
			if (IsForeignKey)
			{
				if (value != null && !ForeignKeyReference.property.PropertyType.IsInstanceOfType(value))
					property.SetValue(model, foreignKeyLoader.Invoke(null, new object[] { convertValue(value, ForeignKeyReference.property.PropertyType) }));
				else
					property.SetValue(model, foreignKeyLoader.Invoke(null, new object[] { value }));
			}
			else
			{
				if (value != null && !property.PropertyType.IsInstanceOfType(value))
					property.SetValue(model, convertValue(value, property.PropertyType));
				else
					property.SetValue(model, value);
			}
		}

		public object GetValue(object model)
		{
			if (IsForeignKey)
				return ForeignKeyReference.GetValue(property.GetValue(model));
			else
				return property.GetValue(model);
		}

		private static ModelProperty GetForeignKeyReference(Type model)
		{
			return (ModelProperty)model.GetField("PrimaryKeyProperty", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
		}

		private static string GetForeignKeyTableName(Type model)
		{
			return (string)model.GetField("TableName", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
		}

		private static object ChangeType(object value, Type targetType) => Convert.ChangeType(value, targetType);

		private static object ToEnumValue(object value, Type targetType) => Enum.ToObject(targetType, value);

		#endregion
	}
}
