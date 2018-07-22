using SystemMath = System.Math;
using SystemRandom = System.Random;

namespace Futilef.V2 {
	public static class Math {
		public const float PI = 3.14159265358979323846f;

		static readonly SystemRandom Rand = new SystemRandom();

		public static float Sqrt(float a) {
			return (float)SystemMath.Sqrt(a);
		}

		public static float Min(float a, float b) {
			return a > b ? b : a;
		}

		public static float Max(float a, float b) {
			return a > b ? a : b;
		}

		public static float Random() {
			return (float)Rand.NextDouble();
		}

		public static float Sin(float a) {
			return (float)SystemMath.Sin(a);
		}

		public static float Cos(float a) {
			return (float)SystemMath.Cos(a);
		}

		public static float Acos(float a) {
			return (float)SystemMath.Acos(a);
		}

		public static float Abs(float a) {
			return a < 0 ? -a : a;
		}
	}
}
