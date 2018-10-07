using Math = System.Math;

namespace Futilef {
	[System.Flags]
	public enum EsType {
		InOut = 0, In = 1, Out = 2,

		Linear = 1 << 2, 
		Quad = 2 << 2, Cubic = 3 << 2, Quart = 4 << 2, Quint = 5 << 2, Sine = 6 << 2, 
		Expo = 7 << 2, Circ = 8 << 2, Back = 9 << 2, Elastic = 10 << 2, Bounce = 11 << 2,
	}

	public static class Es {
		public const float Pi = 3.14159265359f;
		public const float HalfPi = 1.57079632679f;
		public const float TwoPi = 6.28318530718f;

		public static float Linear(float t) {
			return t;
		}

		public static float InQuad(float t) {
			return t * t;
		}
		public static float OutQuad(float t) {
			return t * (2 - t);
		}
		public static float InOutQuad(float t) {
			if ((t *= 2) < 1) return .5f * t * t;
			return .5f * (1 - (t - 1) * (t - 3));
		}

		public static float InCubic(float t) {
			return t * t * t;
		}
		public static float OutCubic(float t) {
			return ((t -= 1) * t * t + 1);
		}
		public static float InOutCubic(float t) {
			if ((t *= 2) < 1) return .5f * t * t * t;
			return .5f * ((t -= 2) * t * t + 2);
		}

		public static float InQuart(float t) {
			return t * t * t * t;
		}
		public static float OutQuart(float t) {
			return 1 - (t -= 1) * t * t * t;
		}
		public static float InOutQuart(float t) {
			if ((t *= 2) < 1) return .5f * t * t * t * t;
			return .5f * (2 - (t -= 2) * t * t * t);
		}

		public static float InQuint(float t) {
			return t * t * t * t * t;
		}
		public static float OutQuint(float t) {
			return ((t -= 1) * t * t * t * t + 1);
		}
		public static float InOutQuint(float t) {
			if ((t *= 2) < 1) return .5f * t * t * t * t * t;
			return .5f * ((t -= 2) * t * t * t * t + 2);
		}

		public static float InSine(float t) {
			return 1 - Math.Cos(t * HalfPi);
		}
		public static float OutSine(float t) {
			return Math.Sin(t * HalfPi);
		}
		public static float InOutSine(float t) {
			return .5f * (1 - Math.Cos(t * Pi));
		}

		public static float InExpo(float t) {
			return Math.Exp(7 * (t - 1));
		}
		public static float OutExpo(float t) {
			return 1 - Math.Exp(-7 * t);
		}
		public static float InOutExpo(float t) {
			if ((t *= 2) < 1) return .5f * Math.Exp(7 * (t - 1));
			return .5f * (2 - Math.Exp(-7 * (t - 1)));
		}

		public static float InCirc(float t) {
			return 1 - Math.Sqrt(1 - t * t);
		}
		public static float OutCirc(float t) {
			return Math.Sqrt(1 - (t -= 1) * t);
		}
		public static float InOutCirc(float t) {
			if ((t *= 2) < 1) return .5f * (1 - Math.Sqrt(1 - t * t));
			return .5f * (Math.Sqrt(1 - (t -= 2) * t) + 1);
		}

		public static float InBack(float t, float s) {
			return t * t * ((s + 1) * t - s);
		}
		public static float OutBack(float t, float s) {
			return (t -= 1) * t * ((s + 1) * t + s) + 1;
		}
		public static float InOutBack(float t, float s) {
			if ((t *= 2) < 1) return .5f * (t * t * (((s *= 1.525f) + 1) * t - s));
			return .5f * ((t -= 2) * t * (((s *= 1.525f) + 1) * t + s) + 2);
		}

		public static float InElastic(float t, float p) {
			return -Math.Exp(7 * (t -= 1)) * Math.Sin((t - p * .25f) * TwoPi / p);
		}
		public static float OutElastic(float t, float p) {
			return Math.Exp(-7 * t) * Math.Sin((t - p * .25f) * TwoPi / p) + 1;
		}
		public static float InOutElastic(float t, float p) {
			if ((t *= 2) < 1) return -.5f * Math.Exp(7 * (t -= 1)) * Math.Sin((t - p * .25f) * TwoPi / p);
			return Math.Exp(-7 * (t -= 1)) * Math.Sin((t - p * .25f) * TwoPi / p) * .5f + 1;
		}

		public static float InBounce(float t) {
			return 1 - Es.easeOutBounce(1 - t);
		}
		public static float OutBounce(float t) {
			if (t < (1 / 2.75f))    return 7.5625f * t * t;
			if (t < (2 / 2.75f))    return 7.5625f * (t -= (1.5f / 2.75f)) * t + .75f;
			if (t < (2.5f / 2.75f)) return 7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f;
								    return 7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f;
		}
		public static float InOutBounce(float t) {
			if ((t *= 2) < 1) return .5f * Es.easeInBounce(t);
			return .5f * (Es.easeOutBounce(t - 1) + 1);
		}
	}
}