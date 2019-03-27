namespace Futilef.V2 {
	public struct Vec4 {
		public float x;
		public float y;
		public float z;
		public float w;

		public Vec4(float x, float y, float z, float w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vec4(float v, float w) {
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = w;
		}

		public Vec4(float v) {
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		public void Zero() {
			x = 0;
			y = 0;
			z = 0;
			w = 0;
		}

		public void One() {
			x = 1;
			y = 1;
			z = 1;
			w = 1;
		}

		public void Set(float v) {
			this.x = v;
			this.y = v;
			this.z = v;
			this.w = v;
		}

		public void Set(float x, float y, float z, float w) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public static Vec4 Blend(Vec4 srcColor, Vec4 dstColor, int srcBlend, int dstBlend, int op) {
			switch (op) {
			case BlendOperator.subtract:
				return BlendHalf(srcColor, dstColor, srcColor, srcBlend) - BlendHalf(srcColor, dstColor, dstColor, dstBlend);
			case BlendOperator.reverseSubtract:
				return BlendHalf(srcColor, dstColor, dstColor, dstBlend) - BlendHalf(srcColor, dstColor, srcColor, srcBlend);
			default:
				return BlendHalf(srcColor, dstColor, srcColor, srcBlend) + BlendHalf(srcColor, dstColor, dstColor, dstBlend);
			}
		}

		public static Vec4 BlendHalf(Vec4 srcColor, Vec4 dstColor, Vec4 color, int blend) {
			var o = new Vec4();
			switch (blend) {
			case BlendFactor.one:              o = color; break;
			case BlendFactor.srcColor:         o = color * srcColor; break;
			case BlendFactor.oneMinusSrcColor: o = color * (new Vec4(1) - srcColor); break;
			case BlendFactor.dstColor:         o = color * dstColor; break;
			case BlendFactor.oneMinusDstColor: o = color * (new Vec4(1) - dstColor); break;
			case BlendFactor.srcAlpha:         o = color * srcColor.w; break;
			case BlendFactor.oneMinusSrcAlpha: o = color * (1 - srcColor.w); break;
			case BlendFactor.dstAlpha:         o = color * dstColor.w; break;
			case BlendFactor.oneMinusDstAlpha: o = color * (1 - dstColor.w); break;
			}
			return o;
		}

		public static Vec4 operator +(Vec4 a, Vec4 b) {
			return new Vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vec4 operator -(Vec4 a, Vec4 b) {
			return new Vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static Vec4 operator *(Vec4 a, Vec4 b) {
			return new Vec4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		public static Vec4 operator *(Vec4 a, float f) {
			return new Vec4(a.x * f, a.y * f, a.z * f, a.w * f);
		}
	}

	public static class BlendFactor {
		public const int zero = 0;
		public const int one = 1;
		public const int srcColor = 2;
		public const int oneMinusSrcColor = 3;
		public const int dstColor = 4;
		public const int oneMinusDstColor = 5;
		public const int srcAlpha = 6;
		public const int oneMinusSrcAlpha = 7;
		public const int dstAlpha = 8;
		public const int oneMinusDstAlpha = 9;
	}

	public static class BlendOperator {
		public const int add = 0;
		public const int subtract = 1;
		public const int reverseSubtract = 2;
	}
}

