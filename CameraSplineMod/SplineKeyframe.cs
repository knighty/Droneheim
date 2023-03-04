namespace Droneheim
{
	public class SplineKeyframe<Type>
	{
		private Type value;
		public Type Value { get { return value; } }

		internal int time;
		public int Time { get { return time; } set { time = value; } }
		
		private EasingMode easingMode;
		public EasingMode Easing { get { return easingMode; } }

		internal SplineKeyframe<Type> next = null;
		public SplineKeyframe<Type> Next { get { return next; } }

		internal SplineKeyframe<Type> previous = null;
		public SplineKeyframe<Type> Previous { get { return previous; } }

		public SplineKeyframe(Type value, int time, EasingMode easingMode = EasingMode.EaseBoth)
		{
			this.value = value;
			this.time = time;
			this.easingMode = easingMode;
		}
	}
}
