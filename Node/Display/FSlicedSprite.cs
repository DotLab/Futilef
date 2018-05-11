using UnityEngine;

using Futilef.Serialization;

namespace Futilef.Node.Display {
	public class FSlicedSprite : FNode {
		public float width, height;

		public readonly Vector2[] localVertices = new Vector2[16];
		public readonly Vector2[] localUvs = new Vector2[16];

		public TpFrame frame;
		public readonly Shader shader;
		public Color32 color = new Color32(255, 255, 255, 255);

		public FSlicedSprite(TpFrame frame, Shader shader) {
			this.frame = frame;
			this.shader = shader;

			width = frame.border.w;
			height = frame.border.h;
		}

		readonly float[] _xs = new float[4], _ys = new float[4];
		public override void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			base.Redraw(ref currentDepth, shouldForceMatricesDirty, shouldForceAlphaDirty);

			var layer = Futilef.Rendering.FRenderer.GetRenderLayer(frame.atlas.texture, shader, Futilef.Rendering.FPrimitiveType.Quad);
			int index = layer.PrimitiveIndexToVertexIndex(layer.GetQuota(9));

			var vertices = layer.vertices;
			var uvs = layer.uvs;
			var colors = layer.colors;

			_xs[0] = frame.quad.x;
			_xs[1] = _xs[0] + frame.border.x;
			_xs[2] = _xs[1] + width;
			_xs[3] = _xs[0] + frame.quad.w - frame.border.w + width;

			_ys[0] = frame.quad.y;
			_ys[1] = _ys[0] + frame.border.y;
			_ys[2] = _ys[1] + height;
			_ys[3] = _ys[0] + frame.quad.h - frame.border.h + height;

			float sizeY = frame.size.y - frame.border.h + height;
			float pivotX = frame.pivot.x / frame.size.x * (frame.size.x - frame.border.w + width);
			float pivotY = (1f - frame.pivot.y / frame.size.y) * (frame.size.y - frame.border.h + height);
			for (int yy = 0; yy < 4; yy++) {
				for (int xx = 0; xx < 4; xx++) {
					localVertices[(yy << 2) + xx].Set(_xs[xx] - pivotX, sizeY - _ys[yy] - pivotY);
				}
			}

			float texW = frame.atlas.size.x, texH = frame.atlas.size.y;
			if (!frame.rotated) {
				/*    x0 x1 x2 x3
				 * y0 00 01 02 03
				 * y1 04 05 06 07
				 * y2 08 09 10 11
				 * y3 12 13 14 15
				 */
				_xs[0] = frame.uv.x;
				_xs[1] = _xs[0] + frame.border.x;
				_xs[2] = _xs[1] + frame.border.w;
				_xs[3] = _xs[0] + frame.uv.w;

				_ys[0] = frame.uv.y;
				_ys[1] = _ys[0] + frame.border.y;
				_ys[2] = _ys[1] + frame.border.h;
				_ys[3] = _ys[0] + frame.uv.h;

				for (int yy = 0; yy < 4; yy++) {
					for (int xx = 0; xx < 4; xx++) {
						localUvs[(yy << 2) + xx].Set(_xs[xx] / texW, 1f - _ys[yy] / texH);
					}
				}
			} else {
				/*    x3 x2 x1 x0
				 * y0 12 08 04 00
				 * y1 13 09 05 01
				 * y2 14 10 06 02
				 * y3 15 11 07 03
				 */
				_xs[0] = frame.uv.x + frame.uv.w;
				_xs[1] = _xs[0] - frame.border.y;
				_xs[2] = _xs[1] - frame.border.h;
				_xs[3] = frame.uv.x;

				_ys[0] = frame.uv.y;
				_ys[1] = _ys[0] + frame.border.x;
				_ys[2] = _ys[1] + frame.border.w;
				_ys[3] = _ys[0] + frame.uv.h;

				for (int yy = 0; yy < 4; yy++) {
					for (int xx = 0; xx < 4; xx++) {
						localUvs[(xx << 2) + yy].Set(_xs[xx] / texW, 1f - _ys[yy] / texH);
					}
				}
			}

			var vertexColor = new Color32(color.r, color.g, color.b, (byte)(_concatenatedAlpha * color.a));
			for (int yy = 0; yy < 3; yy++) {
				for (int xx = 0; xx < 3; xx++) {
					int j = index + ((yy * 3) << 2) + (xx << 2), k1 = (yy << 2) | xx, k2 = ((yy + 1) << 2) | xx;
					_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 0], localVertices[k2]);
					_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 1], localVertices[k1]);
					_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 2], localVertices[k1 + 1]);
					_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 3], localVertices[k2 + 1]);

					uvs[j + 0] = localUvs[k2];
					uvs[j + 1] = localUvs[k1];
					uvs[j + 2] = localUvs[k1 + 1];
					uvs[j + 3] = localUvs[k2 + 1];

					colors[j + 0] = vertexColor;
					colors[j + 1] = vertexColor;
					colors[j + 2] = vertexColor;
					colors[j + 3] = vertexColor;
				}
			}
		}
	}
}

