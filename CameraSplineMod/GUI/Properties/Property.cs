using Droneheim.GUI.Properties.Editors;
using Droneheim.Spline;
using System;
using System.Linq;
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
	public sealed class FloatRangeAttribute : Attribute
	{
		public float Min;
		public float Max;

		public FloatRangeAttribute(float min, float max) { }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class IntRangeAttribute : Attribute
	{
		public int Min;
		public int Max;

		public IntRangeAttribute(int min, int max) { }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class LengthAttribute : Attribute
	{
		public int Length;

		public LengthAttribute(int min, int max) { }
	}

	public interface EditableProperty
	{
		string Name { get; }
	}

	public class KeyframedEditableProperty<T> : EditableProperty
	{
		protected BasicKeyframeList<T> keyframes;
		protected PlaybackController playbackController;

		public string Name => "Property";

		public T Value
		{
			get
			{
				return keyframes.GetValueAt(playbackController.CurrentFrame);
			}
			//set => throw new NotImplementedException();
			set
			{

			}
		}

		public event Action<object> OnChange;

		public KeyframedEditableProperty(BasicKeyframeList<T> keyframes, PlaybackController playbackController)
		{
			if (keyframes == null)
			{
				throw new ArgumentNullException(nameof(keyframes));
			}
			if (playbackController == null)
			{
				throw new ArgumentNullException(nameof(playbackController));
			}
			this.keyframes = keyframes;
			this.playbackController = playbackController;
		}

		public void GoToNextKeyframe()
		{
			throw new NotImplementedException();
		}

		public void GoToPreviousKeyframe()
		{
			throw new NotImplementedException();
		}

		public bool IsKeyframed()
		{
			throw new NotImplementedException();
		}

		public void RemoveKeyframe()
		{
			throw new NotImplementedException();
		}
	}
}