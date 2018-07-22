namespace Futilef.V2 {
	public static class Quat {
		public static float[] Create() {
			var o = new float[4];
			o[3] = 1;
			return o;
		}

		public static float[] Identity(float[] o) {
			o[0] = 0;
			o[1] = 0;
			o[2] = 0;
			o[3] = 1;
			return o;
		}

		public static float[] SetAxisAngle(float[] o, float[] axis, float rad) {
			rad = rad * 0.5f;
			float s = Math.Sin(rad);
			o[0] = s * axis[0];
			o[1] = s * axis[1];
			o[2] = s * axis[2];
			o[3] = Math.Cos(rad);
			return o;
		}

		public static float[] Mul(float[] o, float[] a, float[] b) {
			float ax = a[0], ay = a[1], az = a[2], aw = a[3];
			float bx = b[0], by = b[1], bz = b[2], bw = b[3];
			o[0] = ax * bw + aw * bx + ay * bz - az * by;
			o[1] = ay * bw + aw * by + az * bx - ax * bz;
			o[2] = az * bw + aw * bz + ax * by - ay * bx;
			o[3] = aw * bw - ax * bx - ay * by - az * bz;
			return o;
		}

		public static float[] Slerp(float[] o, float[] a, float[] b, float t) {
			// benchmarks:
			//    http://jsperf.com/quaternion-slerp-implementations
			float ax = a[0], ay = a[1], az = a[2], aw = a[3];
			float bx = b[0], by = b[1], bz = b[2], bw = b[3];
			float omega, cosom, sinom, scale0, scale1;
			// calc cosine
			cosom = ax * bx + ay * by + az * bz + aw * bw;
			// adjust signs (if necessary)
			if (cosom < 0.0) {
				cosom = -cosom;
				bx = -bx;
				by = -by;
				bz = -bz;
				bw = -bw;
			}
			// calculate coefficients
			if ((1.0 - cosom) > 0) {
				// standard case (slerp)
				omega = Math.Acos(cosom);
				sinom = Math.Sin(omega);
				scale0 = Math.Sin((1f - t) * omega) / sinom;
				scale1 = Math.Sin(t * omega) / sinom;
			} else {
				// "from" and "to" quaternions are very close
				//  ... so we can do a linear interpolation
				scale0 = 1f - t;
				scale1 = t;
			}
			// calculate final values
			o[0] = scale0 * ax + scale1 * bx;
			o[1] = scale0 * ay + scale1 * by;
			o[2] = scale0 * az + scale1 * bz;
			o[3] = scale0 * aw + scale1 * bw;
			return o;
		}

		public static float[] Random(float[] o) {
			// Implementation of http://planning.cs.uiuc.edu/node198.html
			// TODO: Calling random 3 times is probably not the fastest solution
			float u1 = Math.Random();
			float u2 = Math.Random();
			float u3 = Math.Random();
			float sqrt1MinusU1 = Math.Sqrt(1f - u1);
			float sqrtU1 = Math.Sqrt(u1);
			o[0] = sqrt1MinusU1 * Math.Sin(2f * Math.PI * u2);
			o[1] = sqrt1MinusU1 * Math.Cos(2f * Math.PI * u2);
			o[2] = sqrtU1 * Math.Sin(2f * Math.PI * u3);
			o[3] = sqrtU1 * Math.Cos(2f * Math.PI * u3);
			return o;
		}

		public static float[] Invert(float[] o, float[] a) {
			float a0 = a[0], a1 = a[1], a2 = a[2], a3 = a[3];
			float dot = a0 * a0 + a1 * a1 + a2 * a2 + a3 * a3;
			if (dot == 0) {
				return null;
			}
			float invDot = 1f / dot;
			// TODO: Would be faster to return [0,0,0,0] immediately if dot == 0
			o[0] = -a0 * invDot;
			o[1] = -a1 * invDot;
			o[2] = -a2 * invDot;
			o[3] = a3 * invDot;
			return o;
		}

		public static float[] FromEuler(float[] o, float x, float y, float z) {
			const float halfToRad = 0.5f * Math.PI / 180f;
			x *= halfToRad;
			y *= halfToRad;
			z *= halfToRad;
			float sx = Math.Sin(x);
			float cx = Math.Cos(x);
			float sy = Math.Sin(y);
			float cy = Math.Cos(y);
			float sz = Math.Sin(z);
			float cz = Math.Cos(z);
			o[0] = sx * cy * cz - cx * sy * sz;
			o[1] = cx * sy * cz + sx * cy * sz;
			o[2] = cx * cy * sz - sx * sy * cz;
			o[3] = cx * cy * cz + sx * sy * sz;
			return o;
		}

		public static string Str(float[] a) {
			return "quat(" + a[0] + ", " + a[1] + ", " + a[2] + ", " + a[3] + ")";
		}
	}
}