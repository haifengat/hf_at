using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	/// <summary>
	/// 自定义属性:类中需继承CustomTypeDescriptor,如需直接改写属性,重新定义继承CustomPropertyDescriptor的类并重写Setvalue
	/// </summary>
	public class CustomTypeDescriptor : ICustomTypeDescriptor
	{
		internal ConcurrentDictionary<string, Property> DicProperties = new ConcurrentDictionary<string, Property>();

		internal void Add(Property value)
		{
			if (value != null)
			{
				this.DicProperties.TryAdd(value.Name, value);
			}
		}

		#region ICustomTypeDescriptor 成员
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptor[] newProps = new PropertyDescriptor[this.DicProperties.Count];
			//for (int i = 0; i < this.dicProperties.Count; i++)
			int i = 0;
			foreach (var v in this.DicProperties)
			{
				//Property prop = (Property)this[i];
				Property prop = v.Value;
				newProps[i++] = new CustomPropertyDescriptor(ref prop, attributes);
			}
			return new PropertyDescriptorCollection(newProps);
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return TypeDescriptor.GetProperties(this, true);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
		#endregion

		/*
		private void Remove(Property value)
		{
			Property t;
			DicProperties.TryRemove(value.Name, out t);
		}
*/
	}

	/// <summary>
	/// 自定义属性类
	/// </summary>
	internal class Property
	{
		private string _category = string.Empty;
		private string _displayname = string.Empty;
		private string _name = string.Empty;
		private bool _visible = true;

		public Property(string sName, object sValue)
		{
			this._name = sName;
			this.Value = sValue;
		}

		public Property(string sName, object sValue, bool sReadonly, bool sVisible)
		{
			this._name = sName;
			this.Value = sValue;
			this.ReadOnly = sReadonly;
			this._visible = sVisible;
		}

		public string Name //获得属性名   
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public string Description //属性显示名称   
		{
			get { return this._displayname; }
			set { this._displayname = value; }
		}

		public TypeConverter Converter //类型转换器，我们在制作下拉列表时需要用到   
		{ get; set; }

		public string Category //属性所属类别   
		{
			get { return this._category; }
			set { this._category = value; }
		}

		public object Value //属性值   
		{ get; set; }

		public bool ReadOnly //是否为只读属性   
		{ get; set; }

		public bool Visible //是否可见   
		{
			get { return this._visible; }
			set { this._visible = value; }
		}

		public virtual object Editor //属性编辑器   
		{ get; set; }
	}

	/// <summary>
	/// 自定义属性描述
	/// </summary>
	internal class CustomPropertyDescriptor : PropertyDescriptor
	{
		protected Property Property;

		public CustomPropertyDescriptor(ref Property myProperty, Attribute[] attrs)
			: base(myProperty.Name, attrs)
		{
			this.Property = myProperty;
		}

		#region PropertyDescriptor 重写方法
		public override Type ComponentType { get { return null; } }

		public override string Description
		{
			get
			{
				//return m_Property.Name;
				return this.Property.Description != "" ? this.Property.Description : this.Property.Name;
			}
		}

		public override string Category { get { return this.Property.Category; } }

		public override string DisplayName
		{
			get
			{
				return this.Property.Name;
				//return m_Property.DisplayName != "" ? m_Property.DisplayName : m_Property.Name;
			}
		}

		public override bool IsReadOnly { get { return this.Property.ReadOnly; } }

		public override TypeConverter Converter { get { return this.Property.Converter; } }

		public override Type PropertyType { get { return this.Property.Value.GetType(); } }

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return this.Property.Value;
		}

		public override void ResetValue(object component)
		{
			//Have to implement   
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void SetValue(object component, object value)
		{
			if (component.GetType().BaseType == typeof(Strategy)) // .FullName == "GFCTP.Strategy")
			{
				FieldInfo fieldInfo = component.GetType().GetField(this.Property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				if (fieldInfo != null)
				{
					fieldInfo.SetValue(component, value);
				}
			}
			this.Property.Value = value;
		}

		public override object GetEditor(Type editorBaseType)
		{
			return this.Property.Editor ?? base.GetEditor(editorBaseType);
		}
		#endregion
	}
}
