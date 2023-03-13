using Droneheim.GUI.Properties.Editors;
using Droneheim.Spline;
using Droneheim.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Droneheim.GUI.Properties
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public abstract class EditablePropertyModifierAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class EditablePropertyAttribute : EditablePropertyModifierAttribute
	{
		public string Name;
		public string Description;

		public EditablePropertyAttribute(string name, string description = "")
		{
			Name = name;
			Description = description;
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class FloatRangeAttribute : EditablePropertyModifierAttribute
	{
		public float Min;
		public float Max;

		public FloatRangeAttribute(float min, float max) { Min = min; Max = max; }

		public float Clamp(float value)
		{
			return Mathf.Clamp(value, Min, Max);
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class IntRangeAttribute : EditablePropertyModifierAttribute
	{
		public int Min;
		public int Max;

		public IntRangeAttribute(int min, int max) { Min = min; Max = max; }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class LengthAttribute : EditablePropertyModifierAttribute
	{
		public int Length;

		public LengthAttribute(int length) { Length = length; }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class EnumAttribute : EditablePropertyModifierAttribute
	{
		public List<string> Values;

		public EnumAttribute(params string[] strings)
		{
			foreach (var str in strings)
				Values.Add(str);
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class EditablePropertyEditorAttribute : EditablePropertyModifierAttribute
	{
		public Type Type { get; set; }
		public Type FactoryType { get; set; }
		public EditablePropertyEditorAttribute(Type type, Type factoryType = null)
		{
			Type = type;
			FactoryType = factoryType;
		}
	}

	public class EditableProperty<T>
	{
		public PropertyInfo PropertyInfo { get; internal set; }
		public IEnumerable<EditablePropertyModifierAttribute> Attributes { get; internal set; }
		public object Object { get; internal set; }

		public T Value
		{
			set { PropertyInfo.SetValue(Object, value); }
			get { return (T)PropertyInfo.GetValue(Object); }
		}

		public EditableProperty(PropertyInfo propertyInfo, object @object)
		{
			PropertyInfo = propertyInfo;
			Object = @object;
			Attributes = propertyInfo.GetCustomAttributes<EditablePropertyModifierAttribute>();
		}

		public A GetAttribute<A>() where A : EditablePropertyModifierAttribute
		{
			foreach (EditablePropertyModifierAttribute modifier in Attributes)
			{
				if (modifier.GetType() == typeof(A))
				{
					return (A)modifier;
				}
			}
			return null;
		}
	}

	public class KeyframeableEditableProperty<T> : EditableProperty<T>
	{
		public BlockTimelineController Timeline { get; set; }

		public KeyframeableEditableProperty(PropertyInfo propertyInfo, object @object, BlockTimelineController timeline) : base(propertyInfo, @object)
		{
			Timeline = timeline;
		}
	}
}