using UnityEngine;

namespace Droneheim
{
	public interface CatmullRomInterpolator<Type>
	{
		Type Remap(float a, float b, Type c, Type d, float u);
	}

	public class CatmullRomCurve<Type>
	{
		public float alpha;
		public CatmullRomInterpolator<Type> interpolator;

		public CatmullRomCurve(CatmullRomInterpolator<Type> interpolator, float alpha = 0.5f)
		{
			this.interpolator = interpolator;
			this.alpha = alpha;
		}

		// Evaluates a point at the given t-value from 0 to 1
		public Type Evaluate(float t, Type p0, Type p1, Type p2, Type p3, float t0, float t1, float t2, float t3)
		{
			/*Vector4 p1Tangent = GetTangent(p0, p1, p2, time0, time1, time2);// * (time2 - time1);
			Vector4 p2Tangent = GetTangent(p1, p2, p3, time1, time2, time3);// * (time2 - time1);

			float t2 = t * t;
			float t3 = t2 * t;

			Vector4 c1 = p1 * (2 * t3 - 3 * t2 + 1);
			Vector4 c2 = p2 * (3 * t2 - 2 * t3);
			Vector4 c3 = p1Tangent * (t3 - 2 * t2 + t);
			Vector4 c4 = p2Tangent * (t3 - t2);

			return c1 + c2 + c3 + c4;*/

			// calculate knots
			/*const float k0 = 0;
			float k1 = GetKnotInterval(p0, p1);
			float k2 = GetKnotInterval(p1, p2) + k1;
			float k3 = GetKnotInterval(p2, p3) + k2;*/

			float k0 = t0;
			float k1 = t1;
			float k2 = t2;
			float k3 = t3;

			//t = k1 + (k2 - k1) * t;

			// evaluate the point
			float u = Mathf.LerpUnclamped(k1, k2, t);
			Type A1 = interpolator.Remap(k0, k1, p0, p1, u);
			Type A2 = interpolator.Remap(k1, k2, p1, p2, u);
			Type A3 = interpolator.Remap(k2, k3, p2, p3, u);
			Type B1 = interpolator.Remap(k0, k2, A1, A2, u);
			Type B2 = interpolator.Remap(k1, k3, A2, A3, u);
			return interpolator.Remap(k1, k2, B1, B2, u);
		}

		public virtual float GetKnotInterval(Type a, Type b)
		{
			return 1;
		}
	}

	public class Vector3CatmullRomInterpolator : CatmullRomInterpolator<Vector3>
	{
		public Vector3 Remap(float a, float b, Vector3 c, Vector3 d, float u)
		{
			return Vector3.LerpUnclamped(c, d, (u - a) / (b - a));
		}
	}

	public class Vector4CatmullRomInterpolator : CatmullRomInterpolator<Vector4>
	{
		public Vector4 Remap(float a, float b, Vector4 c, Vector4 d, float u)
		{
			return Vector4.LerpUnclamped(c, d, (u - a) / (b - a));
		}
	}

	public class QuaternionCatmullRomInterpolator : CatmullRomInterpolator<Quaternion>
	{
		public Quaternion Remap(float a, float b, Quaternion c, Quaternion d, float u)
		{
			return Quaternion.SlerpUnclamped(c, d, (u - a) / (b - a));
		}
	}
}
