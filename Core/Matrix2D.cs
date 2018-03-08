using System;
using UnityEngine;

namespace Futilef.Core {

	/**
	 * m00 m01 m02
	 * m10 m11 m12
	 * 0   0   1
	 */
	public class Matrix2D {
		public static readonly Matrix2D Identity = new Matrix2D();

		// Useful for doing calulations without allocating a new matrix every time
		public static readonly Matrix2D TempMatrix = new Matrix2D();

		public float m00 = 1;
		public float m01 = 0;
		public float m02 = 0;

		public float m10 = 0;
		public float m11 = 1;
		public float m12 = 0;

		public static Vector2 operator*(Matrix2D m, Vector2 v) {
			return m.Transform2D(v);
		}

		// Creates a new mat2d initialized with values from an existing matrix
		public Matrix2D Clone() {
			var res = new Matrix2D();

			res.m00 = m00;
			res.m01 = m01;
			res.m02 = m02;

			res.m10 = m10;
			res.m11 = m11;
			res.m12 = m12;

			return res;
		}

		// Copy the values from one mat2d to another
		public void FromCopy(Matrix2D a) {
			m00 = a.m00;
			m01 = a.m01;
			m02 = a.m02;

			m10 = a.m10;
			m11 = a.m11;
			m12 = a.m12;
		}

		public void FromIdentity() {
			m00 = 1;
			m01 = 0;
			m02 = 0;

			m10 = 0;
			m11 = 1;
			m12 = 0;
		}

		/**
		 * m00 m01 m02   x   x * m00 + y * m01 + m02
		 * m10 m11 m12 . y = x * m10 + y * m11 + m12
		 *  0   0   1    1   1
		 */
		public Vector2 Transform2D(float x, float y) {
			return new Vector2(
				x * m00 + y * m01 + m02,
				x * m10 + y * m11 + m12); 
		}

		public Vector2 Transform2D(Vector2 v) {
			return Transform2D(v.x, v.y);
		}

		public Vector3 Transform3D(float x, float y, float z = 0) {
			return new Vector3(
				x * m00 + y * m01 + m02,
				x * m10 + y * m11 + m12,
				z);	
		}

		public Vector3 Transform3D(Vector2 v, float z = 0) {
			return Transform3D(v.x, v.y, z);
		}

		public void ApplyTransform3D(ref Vector3 o, Vector2 v, float z = 0) {
			o.x = v.x * m00 + v.y * m01 + m02;
			o.y = v.x * m10 + v.y * m11 + m12;
			o.z = z;
		}

		/**
		 * 1 0 x   m00 m01 m02   m00 m01 (m02 + x)
		 * 0 1 y . m10 m11 m12 = m10 m11 (m12 + y)
		 * 0 0 1   0   0   1     0   0   1
		 */
		public void Translate(float x, float y) {
			m02 += x;
			m12 += y;
		}

		public void Translate(Vector2 v) {
			Translate(v.x, v.y);
		}

		/**
		 * x 0 0   m00 m01 m02   (m00 * x) (m01 * x) (m02 * x)
		 * 0 y 0 . m10 m11 m12 = (m10 * y) (m11 * y) (m12 * y)
		 * 0 0 1   0   0   1     0         0         1
		 */
		public void Scale(float x, float y) {
			m00 *= x;
			m01 *= x;
			m02 *= x;

			m10 *= y;
			m11 *= y;
			m12 *= y;
		}

		public void Scale(Vector2 v) {
			Scale(v.x, v.y);
		}

		/**
		 * cos -sin 0   m00 m01 m02   (m00 * cos - m10 * sin) (m01 * cos - m11 * sin) (m02 * cos - m12 * sin)
		 * sin cos  0 . m10 m11 m12 = (m00 * sin + m10 * cos) (m01 * sin + m11 * cos) (m02 * sin + m12 * cos)
		 * 0   0    1   0   0   1     0                       0                       1
		 */
		public void Rotate(float rad) {
			float sin = Mathf.Sin(rad), cos = Mathf.Cos(rad);

			float a00 = m00, a01 = m01, a02 = m02;
			float a10 = m10, a11 = m11, a12 = m12;

			m00 = a00 * cos - a10 * sin;
			m01 = a01 * cos - a11 * sin;
			m02 = a02 * cos - a12 * sin;

			m10 = a00 * sin + a10 * cos;
			m11 = a01 * sin + a11 * cos;
			m12 = a02 * sin + a12 * cos;
		}

		/**
		 * b00 b01 b02   a00 a01 a02   (a00 * b00 + a10 * b01) (a01 * b00 + a11 * b01) (a00 * b00 + a10 * b01 + b02)
		 * b10 b11 b12 . a10 a11 a12 = (a00 * b10 + a10 * b11) (a01 * b10 + a11 * b11) (a00 * b10 + a10 * b11 + b12)
		 * 0   0   1     0   0   1     0                       0                       1
		 */
		public void Multiply(Matrix2D b) {
			float a00 = m00, a01 = m01, a02 = m02;
			float a10 = m10, a11 = m11, a12 = m12;

			m00 = a00 * b.m00 + a10 * b.m01;
			m01 = a01 * b.m00 + a11 * b.m01;
			m02 = a02 * b.m00 + a12 * b.m01 + b.m02;

			m10 = a00 * b.m10 + a10 * b.m11;
			m11 = a01 * b.m10 + a11 * b.m11;
			m12 = a02 * b.m10 + a12 * b.m11 + b.m12;	
		}

		public void FromMultiply(Matrix2D a, Matrix2D b) {
			// a and b cannot be this!
			if (this == a || this == b) throw new Exception("Cannot multiply a b");

			m00 = a.m00 * b.m00 + a.m10 * b.m01;
			m01 = a.m01 * b.m00 + a.m11 * b.m01;
			m02 = a.m02 * b.m00 + a.m12 * b.m01 + b.m02;

			m10 = a.m00 * b.m10 + a.m10 * b.m11;
			m11 = a.m01 * b.m10 + a.m11 * b.m11;
			m12 = a.m02 * b.m10 + a.m12 * b.m11 + b.m12;	
		}

		public void Invert() {
			float a00 = m00, a01 = m01, a02 = m02;
			float a10 = m10, a11 = m11, a12 = m12;

			float det = 1.0f / (m00 * m11 - m10 * m01);		

			m00 = a11 * det;
			m02 = (a01 * a12 - a11 * a02) * det;
			m01 = -a01 * det;

			m10 = -a10 * det;
			m11 = a00 * det;
			m12 = -(a00 * a12 - a10 * a02) * det;
		}

		public void FromInvert(Matrix2D a) {
			float det = 1.0f / (a.m00 * a.m11 - a.m10 * a.m01);		

			m00 = a.m11 * det;
			m01 = -a.m01 * det;
			m02 = (a.m01 * a.m12 - a.m11 * a.m02) * det;

			m10 = -a.m10 * det;
			m11 = a.m00 * det;
			m12 = -(a.m00 * a.m12 - a.m10 * a.m02) * det;
		}

		/**
		 * 1 0 x   cos -sin 0   sx 0 0   1 0 0   (sx * cos) (sy * -sin) x
		 * 0 1 y . sin cos  0 . 0 sy 0 . 0 1 0 = (sx * sin) (sy * cos)  y
		 * 0 0 1   0   0    1   0 0  1   0 0 1   0          0           1
		 */
		public void FromScalingRotationTranslation(float x, float y, float sx, float sy, float rad) {
			// scales then rotates then translates
			float sin = Mathf.Sin(rad), cos = Mathf.Cos(rad);

			m00 = sx * cos;
			m01 = sy * -sin;
			m02 = x;

			m10 = sx * sin;
			m11 = sy * cos;
			m12 = y;
		}

		public float GetScalingX() {
			return Mathf.Sqrt(m00 * m00 + m10 * m10);
		}

		public float GetScalingY() {
			return Mathf.Sqrt(m01 * m01 + m11 * m11);
		}

		public float GetRotation() {
			Vector2 newVector = Transform2D(1, 0);
			return Mathf.Atan2(newVector.y - m12, newVector.x - m02);
		}
			
		public override string ToString() {
			return string.Format("[FMatrix: m00={0}, m01={1}, m02={2}, m10={3}, m11={4}, m12={5}]", m00, m01, m02, m10, m11, m12);
		}
	}
}

