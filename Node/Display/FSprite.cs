using UnityEngine;

using Futilef.Serialization;

namespace Futilef.Node.Display {
	public class FSprite : FNode {
		public TpFrame frame;
		public readonly Shader shader;
		public Color32 color = new Color32(255, 255, 255, 255);

		public FSprite(TpFrame frame, Shader shader) {
			this.frame = frame;
			this.shader = shader;
		}

		public override void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			base.Redraw(ref currentDepth, shouldForceMatricesDirty, shouldForceAlphaDirty);

			var layer = Futilef.Rendering.FRenderer.GetRenderLayer(frame.atlas.texture, shader, Futilef.Rendering.FPrimitiveType.Quad);
			int index = layer.PrimitiveIndexToVertexIndex(layer.GetQuota(1));

			var vertices = layer.vertices;
			var uvs = layer.uvs;
			var colors = layer.colors;

			_concatenatedMatrix.ApplyTransform3D(ref vertices[index + 0], frame.rectLeftBottom);
			_concatenatedMatrix.ApplyTransform3D(ref vertices[index + 1], frame.rectLeftTop);
			_concatenatedMatrix.ApplyTransform3D(ref vertices[index + 2], frame.rectRightTop);
			_concatenatedMatrix.ApplyTransform3D(ref vertices[index + 3], frame.rectRightBottom);

			uvs[index + 0] = frame.uvLeftBottom;
			uvs[index + 1] = frame.uvLeftTop;
			uvs[index + 2] = frame.uvRightTop;
			uvs[index + 3] = frame.uvRightBottom;

			var vertexColor = new Color32(color.r, color.g, color.b, (byte)(_concatenatedAlpha * color.a));
			colors[index + 0] = vertexColor;
			colors[index + 1] = vertexColor;
			colors[index + 2] = vertexColor;
			colors[index + 3] = vertexColor;
		}
	}
}

