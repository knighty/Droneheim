namespace Droneheim
{
	public class SplineKeyframe<Type> : Keyframe
	{
		private Type value;
		public Type Value { get { return value; } set { this.value = value; } }

		internal int frame;
		public int Frame { get { return frame; } set { frame = value; } }
		
		private EasingMode easingMode;
		public EasingMode Easing { get { return easingMode; } }

		internal SplineKeyframe<Type> next = null;
		public SplineKeyframe<Type> Next { get { return next; } }

		internal SplineKeyframe<Type> previous = null;
		public SplineKeyframe<Type> Previous { get { return previous; } }

		public SplineKeyframe(Type value, int frame, EasingMode easingMode = EasingMode.EaseBoth)
		{
			this.value = value;
			this.frame = frame;
			this.easingMode = easingMode;
		}
	}
}
