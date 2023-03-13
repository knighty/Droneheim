namespace Droneheim
{
	public class StringKeyframes : InterpolatedKeyframeList<string>, Interpolator<string>
	{
		public StringKeyframes(string defaultValue) : base(defaultValue)
		{
		}

		protected override Interpolator<string> Interpolator { get { return this; } }
		protected override KeyframeListMagnitude<string> MagnitudeCalculator => null;

		public string Empty { get { return ""; } }

		public string Interpolate(string a, string b, float t)
		{
			return a;
		}
	}
}
