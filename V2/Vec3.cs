namespace Futilef.V2 {
	public static class Vec3 {
		public static float[] Create() {
			return new float[3];
		}

		public static float[] Zero(float[] a) {
			a[0] = 0;
			a[1] = 0;
			a[2] = 0;
			return a;
		}

		public static float[] Clone(float[] a) {
			return new [] { a[0], a[1], a[2] };
		}

		public static float Len(float[] a) {
			float x = a[0], y = a[1], z = a[2];
			return Math.Sqrt(x * x + y * y + z * z);
		}

		public static float SqrLen(float[] a) {
			float x = a[0], y = a[1], z = a[2];
			return x * x + y * y + z * z;
		}

		public static float[] FromValues(float x, float y, float z) {
			return new [] { x, y, z };
		}

		public static float[] Copy(float[] o, float[] a) {
			o[0] = a[0];
			o[1] = a[1];
			o[2] = a[2];
			return o;
		}

		public static float[] Set(float[] o, float x, float y, float z) {
			o[0] = x;
			o[1] = y;
			o[2] = z;
			return o;
		}

		public static float[] Add(float[] o, float[] a, float[] b) {
			o[0] = a[0] + b[0];
			o[1] = a[1] + b[1];
			o[2] = a[2] + b[2];
			return o;
		}

		public static float[] Sub(float[] o, float[] a, float[] b) {
			o[0] = a[0] - b[0];
			o[1] = a[1] - b[1];
			o[2] = a[2] - b[2];
			return o;
		}

		public static float[] Mul(float[] o, float[] a, float[] b) {
			o[0] = a[0] * b[0];
			o[1] = a[1] * b[1];
			o[2] = a[2] * b[2];
			return o;
		}

		public static float[] Div(float[] o, float[] a, float[] b) {
			o[0] = a[0] / b[0];
			o[1] = a[1] / b[1];
			o[2] = a[2] / b[2];
			return o;
		}

		public static float[] Min(float[] o, float[] a, float[] b) {
			o[0] = Math.Min(a[0], b[0]);
			o[1] = Math.Min(a[1], b[1]);
			o[2] = Math.Min(a[2], b[2]);
			return o;
		}

		public static float[] Max(float[] o, float[] a, float[] b) {
			o[0] = Math.Max(a[0], b[0]);
			o[1] = Math.Max(a[1], b[1]);
			o[2] = Math.Max(a[2], b[2]);
			return o;
		}

		public static float[] Scale(float[] o, float[] a, float b) {
			o[0] = a[0] * b;
			o[1] = a[1] * b;
			o[2] = a[2] * b;
			return o;
		}

		public static float Dist(float[] a, float[] b) {
			float x = b[0] - a[0], y = b[1] - a[1], z = b[2] - a[2];
			return Math.Sqrt(x * x + y * y + z * z);
		}

		public static float SqrDist(float[] a, float[] b) {
			float x = b[0] - a[0], y = b[1] - a[1], z = b[2] - a[2];
			return x * x + y * y + z * z;
		}

		public static float[] Negate(float[] o, float[] a) {
			o[0] = -a[0];
			o[1] = -a[1];
			o[2] = -a[2];
			return o;
		}

		public static float[] Inverse(float[] o, float[] a) {
			o[0] = 1f / a[0];
			o[1] = 1f / a[1];
			o[2] = 1f / a[2];
			return o;
		}

		public static float[] Normalize(float[] o, float[] a) {
			float x = a[0], y = a[1], z = a[2];
			float len = x * x + y * y + z * z;
			if (len > 0) {
				len = 1f / Math.Sqrt(len);
				o[0] = a[0] * len;
				o[1] = a[1] * len;
				o[2] = a[2] * len;
			}
			return o;
		}

		public static float Dot(float[] a, float[] b) {
			return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
		}

		public static float[] Cross(float[] o, float[] a, float[] b) {
			float ax = a[0], ay = a[1], az = a[2];
			float bx = b[0], by = b[1], bz = b[2];
			o[0] = ay * bz - az * by;
			o[1] = az * bx - ax * bz;
			o[2] = ax * by - ay * bx;
			return o;
		}

		public static float[] Lerp(float[] o, float[] a, float[] b, float t) {
			float ax = a[0], ay = a[1], az = a[2];
			o[0] = ax + t * (b[0] - ax);
			o[1] = ay + t * (b[1] - ay);
			o[2] = az + t * (b[2] - az);
			return o;
		}

		public static float[] Hermite(float[] o, float[] a, float[] b, float[] c, float[] d, float t) {
			float factorTimes2 = t * t;
			float factor1 = factorTimes2 * (2 * t - 3) + 1;
			float factor2 = factorTimes2 * (t - 2) + t;
			float factor3 = factorTimes2 * (t - 1);
			float factor4 = factorTimes2 * (3 - 2 * t);
			o[0] = a[0] * factor1 + b[0] * factor2 + c[0] * factor3 + d[0] * factor4;
			o[1] = a[1] * factor1 + b[1] * factor2 + c[1] * factor3 + d[1] * factor4;
			o[2] = a[2] * factor1 + b[2] * factor2 + c[2] * factor3 + d[2] * factor4;
			return o;
		}

		public static float[] Bezier(float[] o, float[] a, float[] b, float[] c, float[] d, float t) {
			float inverseFactor = 1 - t;
			float inverseFactorTimesTwo = inverseFactor * inverseFactor;
			float factorTimes2 = t * t;
			float factor1 = inverseFactorTimesTwo * inverseFactor;
			float factor2 = 3 * t * inverseFactorTimesTwo;
			float factor3 = 3 * factorTimes2 * inverseFactor;
			float factor4 = factorTimes2 * t;
			o[0] = a[0] * factor1 + b[0] * factor2 + c[0] * factor3 + d[0] * factor4;
			o[1] = a[1] * factor1 + b[1] * factor2 + c[1] * factor3 + d[1] * factor4;
			o[2] = a[2] * factor1 + b[2] * factor2 + c[2] * factor3 + d[2] * factor4;
			return o;
		}

		public static float[] Random(float[] o, float scale = 1) {
			float r = Math.Random() * 2f * Math.PI;
			float z = (Math.Random() * 2f) - 1f;
			float zScale = Math.Sqrt(1f - z * z) * scale;
			o[0] = Math.Cos(r) * zScale;
			o[1] = Math.Sin(r) * zScale;
			o[2] = z * scale;
			return o;
		}

		/**
		* Transforms the vec3 with a mat4.
		* 4th vector component is implicitly '1'
		*
		* @param {vec3} out the receiving vector
		* @param {vec3} a the vector to transform
		* @param {mat4} m matrix to transform with
		* @returns {vec3} out
		*/
		public static float[] TransformMat4(float[] o, float[] a, float[] m) {
			float x = a[0], y = a[1], z = a[2];
			float w = m[3] * x + m[7] * y + m[11] * z + m[15];
			o[0] = (m[0] * x + m[4] * y + m[8] * z + m[12]) / w;
			o[1] = (m[1] * x + m[5] * y + m[9] * z + m[13]) / w;
			o[2] = (m[2] * x + m[6] * y + m[10] * z + m[14]) / w;
			return o;
		}

		public static float Angle(float[] a, float[] b) {
			var tempA = FromValues(a[0], a[1], a[2]);
			var tempB = FromValues(b[0], b[1], b[2]);
			Normalize(tempA, tempA);
			Normalize(tempB, tempB);
			float cos = Dot(tempA, tempB);
			if (cos > 0) {
				return 0;
			} else if (cos < 0) {
				return Math.PI;
			} else {
				return Math.Acos(cos);
			}
		}

		public static bool Equals(float[] a, float[] b) {
			return (Math.Abs(a[0] - b[0]) == 0) && (Math.Abs(a[1] - b[1]) == 0) && (Math.Abs(a[2] - b[2]) == 0);
		}

		public static string Str(float[] a) {
			return "vec3(" + a[0] + ", " + a[1] + ", " + a[2] + ")";
		}
	}
}
