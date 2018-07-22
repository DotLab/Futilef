namespace Futilef.V2 {
	public static class Mat4 {
		public static float[] Create() {
			var o = new float[16];
			o[0] = 1;
			o[5] = 1;
			o[10] = 1;
			o[15] = 1;
			return o;
		}

		public static float[] Clone(float[] a) {
			var o = new float[16];
			o[0] = a[0];
			o[1] = a[1];
			o[2] = a[2];
			o[3] = a[3];
			o[4] = a[4];
			o[5] = a[5];
			o[6] = a[6];
			o[7] = a[7];
			o[8] = a[8];
			o[9] = a[9];
			o[10] = a[10];
			o[11] = a[11];
			o[12] = a[12];
			o[13] = a[13];
			o[14] = a[14];
			o[15] = a[15];
			return o;
		}

		public static float[] Copy(float[] o, float[] a) {
			o[0] = a[0];
			o[1] = a[1];
			o[2] = a[2];
			o[3] = a[3];
			o[4] = a[4];
			o[5] = a[5];
			o[6] = a[6];
			o[7] = a[7];
			o[8] = a[8];
			o[9] = a[9];
			o[10] = a[10];
			o[11] = a[11];
			o[12] = a[12];
			o[13] = a[13];
			o[14] = a[14];
			o[15] = a[15];
			return o;
		}

		public static float[] FromValues(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33) {
			var o = new float[16];
			o[0] = m00;
			o[1] = m01;
			o[2] = m02;
			o[3] = m03;
			o[4] = m10;
			o[5] = m11;
			o[6] = m12;
			o[7] = m13;
			o[8] = m20;
			o[9] = m21;
			o[10] = m22;
			o[11] = m23;
			o[12] = m30;
			o[13] = m31;
			o[14] = m32;
			o[15] = m33;
			return o;
		}

		public static float[] Identity(float[] o) {
			o[0] = 1;
			o[1] = 0;
			o[2] = 0;
			o[3] = 0;
			o[4] = 0;
			o[5] = 1;
			o[6] = 0;
			o[7] = 0;
			o[8] = 0;
			o[9] = 0;
			o[10] = 1;
			o[11] = 0;
			o[12] = 0;
			o[13] = 0;
			o[14] = 0;
			o[15] = 1;
			return o;
		}

		public static float[] Transpose(float[] o, float[] a) {
			// If we are transposing ourselves we can skip a few steps but have to cache some values
			if (o == a) {
				float a01 = a[1], a02 = a[2], a03 = a[3];
				float a12 = a[6], a13 = a[7];
				float a23 = a[11];
				o[1] = a[4];
				o[2] = a[8];
				o[3] = a[12];
				o[4] = a01;
				o[6] = a[9];
				o[7] = a[13];
				o[8] = a02;
				o[9] = a12;
				o[11] = a[14];
				o[12] = a03;
				o[13] = a13;
				o[14] = a23;
			} else {
				o[0] = a[0];
				o[1] = a[4];
				o[2] = a[8];
				o[3] = a[12];
				o[4] = a[1];
				o[5] = a[5];
				o[6] = a[9];
				o[7] = a[13];
				o[8] = a[2];
				o[9] = a[6];
				o[10] = a[10];
				o[11] = a[14];
				o[12] = a[3];
				o[13] = a[7];
				o[14] = a[11];
				o[15] = a[15];
			}
			return o;
		}

		public static float[] Invert(float[] o, float[] a) {
			float a00 = a[0], a01 = a[1], a02 = a[2], a03 = a[3];
			float a10 = a[4], a11 = a[5], a12 = a[6], a13 = a[7];
			float a20 = a[8], a21 = a[9], a22 = a[10], a23 = a[11];
			float a30 = a[12], a31 = a[13], a32 = a[14], a33 = a[15];
			float b00 = a00 * a11 - a01 * a10;
			float b01 = a00 * a12 - a02 * a10;
			float b02 = a00 * a13 - a03 * a10;
			float b03 = a01 * a12 - a02 * a11;
			float b04 = a01 * a13 - a03 * a11;
			float b05 = a02 * a13 - a03 * a12;
			float b06 = a20 * a31 - a21 * a30;
			float b07 = a20 * a32 - a22 * a30;
			float b08 = a20 * a33 - a23 * a30;
			float b09 = a21 * a32 - a22 * a31;
			float b10 = a21 * a33 - a23 * a31;
			float b11 = a22 * a33 - a23 * a32;
			// Calculate the determinant
			float det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;
			if (det == 0) {
				return null;
			}
			det = 1f / det;
			o[0] = (a11 * b11 - a12 * b10 + a13 * b09) * det;
			o[1] = (a02 * b10 - a01 * b11 - a03 * b09) * det;
			o[2] = (a31 * b05 - a32 * b04 + a33 * b03) * det;
			o[3] = (a22 * b04 - a21 * b05 - a23 * b03) * det;
			o[4] = (a12 * b08 - a10 * b11 - a13 * b07) * det;
			o[5] = (a00 * b11 - a02 * b08 + a03 * b07) * det;
			o[6] = (a32 * b02 - a30 * b05 - a33 * b01) * det;
			o[7] = (a20 * b05 - a22 * b02 + a23 * b01) * det;
			o[8] = (a10 * b10 - a11 * b08 + a13 * b06) * det;
			o[9] = (a01 * b08 - a00 * b10 - a03 * b06) * det;
			o[10] = (a30 * b04 - a31 * b02 + a33 * b00) * det;
			o[11] = (a21 * b02 - a20 * b04 - a23 * b00) * det;
			o[12] = (a11 * b07 - a10 * b09 - a12 * b06) * det;
			o[13] = (a00 * b09 - a01 * b07 + a02 * b06) * det;
			o[14] = (a31 * b01 - a30 * b03 - a32 * b00) * det;
			o[15] = (a20 * b03 - a21 * b01 + a22 * b00) * det;
			return o;
		}

		public static float Determinant(float[] a) {
			float a00 = a[0], a01 = a[1], a02 = a[2], a03 = a[3];
			float a10 = a[4], a11 = a[5], a12 = a[6], a13 = a[7];
			float a20 = a[8], a21 = a[9], a22 = a[10], a23 = a[11];
			float a30 = a[12], a31 = a[13], a32 = a[14], a33 = a[15];
			float b00 = a00 * a11 - a01 * a10;
			float b01 = a00 * a12 - a02 * a10;
			float b02 = a00 * a13 - a03 * a10;
			float b03 = a01 * a12 - a02 * a11;
			float b04 = a01 * a13 - a03 * a11;
			float b05 = a02 * a13 - a03 * a12;
			float b06 = a20 * a31 - a21 * a30;
			float b07 = a20 * a32 - a22 * a30;
			float b08 = a20 * a33 - a23 * a30;
			float b09 = a21 * a32 - a22 * a31;
			float b10 = a21 * a33 - a23 * a31;
			float b11 = a22 * a33 - a23 * a32;
			// Calculate the determinant
			return b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;
		}

		public static float[] Mul(float[] o, float[] a, float[] b) {
			float a00 = a[0], a01 = a[1], a02 = a[2], a03 = a[3];
			float a10 = a[4], a11 = a[5], a12 = a[6], a13 = a[7];
			float a20 = a[8], a21 = a[9], a22 = a[10], a23 = a[11];
			float a30 = a[12], a31 = a[13], a32 = a[14], a33 = a[15];
			// Cache only the current line of the second matrix
			float b0 = b[0], b1 = b[1], b2 = b[2], b3 = b[3];
			o[0] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
			o[1] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
			o[2] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
			o[3] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
			b0 = b[4];
			b1 = b[5];
			b2 = b[6];
			b3 = b[7];
			o[4] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
			o[5] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
			o[6] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
			o[7] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
			b0 = b[8];
			b1 = b[9];
			b2 = b[10];
			b3 = b[11];
			o[8] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
			o[9] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
			o[10] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
			o[11] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
			b0 = b[12];
			b1 = b[13];
			b2 = b[14];
			b3 = b[15];
			o[12] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
			o[13] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
			o[14] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
			o[15] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
			return o;
		}

		/**
		* Creates a matrix from a quaternion rotation, vector translation and vector scale
		* This is equivalent to (but much faster than):
		*
		*     mat4.identity(dest);
		*     mat4.translate(dest, vec);
		*     let quatMat = mat4.create();
		*     quat4.toMat4(quat, quatMat);
		*     mat4.multiply(dest, quatMat);
		*     mat4.scale(dest, scale)
		*
		* @param {mat4} out mat4 receiving operation result
		* @param {quat4} q Rotation quaternion
		* @param {vec3} v Translation vector
		* @param {vec3} s Scaling vector
		* @returns {mat4} out
		*/
		public static float[] FromRotationTranslationScale(float[] o, float[] q, float[] v, float[] s) {
			// Quaternion math
			float x = q[0], y = q[1], z = q[2], w = q[3];
			float x2 = x + x;
			float y2 = y + y;
			float z2 = z + z;
			float xx = x * x2;
			float xy = x * y2;
			float xz = x * z2;
			float yy = y * y2;
			float yz = y * z2;
			float zz = z * z2;
			float wx = w * x2;
			float wy = w * y2;
			float wz = w * z2;
			float sx = s[0];
			float sy = s[1];
			float sz = s[2];
			o[0] = (1 - (yy + zz)) * sx;
			o[1] = (xy + wz) * sx;
			o[2] = (xz - wy) * sx;
			o[3] = 0;
			o[4] = (xy - wz) * sy;
			o[5] = (1 - (xx + zz)) * sy;
			o[6] = (yz + wx) * sy;
			o[7] = 0;
			o[8] = (xz + wy) * sz;
			o[9] = (yz - wx) * sz;
			o[10] = (1 - (xx + yy)) * sz;
			o[11] = 0;
			o[12] = v[0];
			o[13] = v[1];
			o[14] = v[2];
			o[15] = 1;
			return o;
		}

		public static float[] FromRotationTranslationScale(float[] o, float[] q, float vx, float vy, float vz, float sx, float sy, float sz) {
			// Quaternion math
			float x = q[0], y = q[1], z = q[2], w = q[3];
			float x2 = x + x;
			float y2 = y + y;
			float z2 = z + z;
			float xx = x * x2;
			float xy = x * y2;
			float xz = x * z2;
			float yy = y * y2;
			float yz = y * z2;
			float zz = z * z2;
			float wx = w * x2;
			float wy = w * y2;
			float wz = w * z2;
			o[0] = (1 - (yy + zz)) * sx;
			o[1] = (xy + wz) * sx;
			o[2] = (xz - wy) * sx;
			o[3] = 0;
			o[4] = (xy - wz) * sy;
			o[5] = (1 - (xx + zz)) * sy;
			o[6] = (yz + wx) * sy;
			o[7] = 0;
			o[8] = (xz + wy) * sz;
			o[9] = (yz - wx) * sz;
			o[10] = (1 - (xx + yy)) * sz;
			o[11] = 0;
			o[12] = vx;
			o[13] = vy;
			o[14] = vz;
			o[15] = 1;
			return o;
		}

		public static string Str(float[] a) {
			return "mat4(" + a[0] + ", " + a[1] + ", " + a[2] + ", " + a[3] + ", " +
			a[4] + ", " + a[5] + ", " + a[6] + ", " + a[7] + ", " +
			a[8] + ", " + a[9] + ", " + a[10] + ", " + a[11] + ", " +
			a[12] + ", " + a[13] + ", " + a[14] + ", " + a[15] + ")";
		}
	}
}