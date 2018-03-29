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

		public override void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			base.Redraw(ref currentDepth, shouldForceMatricesDirty, shouldForceAlphaDirty);

			var layer = Futilef.Rendering.FRenderer.GetRenderLayer(frame.atlas.texture, shader, Futilef.Rendering.FPrimitiveType.Quad);
			int index = layer.PrimitiveIndexToVertexIndex(layer.GetQuota(9));

			var vertices = layer.vertices;
			var uvs = layer.uvs;
			var colors = layer.colors;

			var xs = new float[4];
			var ys = new float[4];
			xs[0] = frame.quad.x;
			xs[1] = xs[0] + frame.border.x;
			xs[2] = xs[1] + width;
			xs[3] = xs[0] + frame.quad.w - frame.border.w + width;

			ys[0] = frame.quad.y;
			ys[1] = ys[0] + frame.border.y;
			ys[2] = ys[1] + height;
			ys[3] = ys[0] + frame.quad.h - frame.border.h + height;

			float sizeY = frame.size.y - frame.border.h + height;
			float pivotX = frame.pivot.x / frame.size.x * (frame.size.x - frame.border.w + width);
			float pivotY = (1f - frame.pivot.y / frame.size.y) * (frame.size.y - frame.border.h + height);
			for (int yy = 0; yy < 4; yy++) {
				for (int xx = 0; xx < 4; xx++) {
					localVertices[(yy << 2) + xx].Set(xs[xx] - pivotX, sizeY - ys[yy] - pivotY);
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
				xs[0] = frame.uv.x;
				xs[1] = xs[0] + frame.border.x;
				xs[2] = xs[1] + frame.border.w;
				xs[3] = xs[0] + frame.uv.w;

				ys[0] = frame.uv.y;
				ys[1] = ys[0] + frame.border.y;
				ys[2] = ys[1] + frame.border.h;
				ys[3] = ys[0] + frame.uv.h;

				for (int yy = 0; yy < 4; yy++) {
					for (int xx = 0; xx < 4; xx++) {
						localUvs[(yy << 2) + xx].Set(xs[xx] / texW, 1f - ys[yy] / texH);
					}
				}
			} else {
				/*    x3 x2 x1 x0
				 * y0 12 08 04 00
				 * y1 13 09 05 01
				 * y2 14 10 06 02
				 * y3 15 11 07 03
				 */
				xs[0] = frame.uv.x + frame.uv.w;
				xs[1] = xs[0] - frame.border.y;
				xs[2] = xs[1] - frame.border.h;
				xs[3] = frame.uv.x;

				ys[0] = frame.uv.y;
				ys[1] = ys[0] + frame.border.x;
				ys[2] = ys[1] + frame.border.w;
				ys[3] = ys[0] + frame.uv.h;

				for (int yy = 0; yy < 4; yy++) {
					for (int xx = 0; xx < 4; xx++) {
						localUvs[(xx << 2) + yy].Set(xs[xx] / texW, 1f - ys[yy] / texH);
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

