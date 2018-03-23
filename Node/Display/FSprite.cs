using UnityEngine;

using Futilef.Serialization;

namespace Futilef.Node.Display {
	public class FSprite : FNode {
		readonly TpFrame _frame;
		readonly Shader _shader;

		public FSprite(TpFrame frame, Shader shader) {
			_frame = frame;
			_shader = shader;
		}

		public override void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			base.Redraw(ref currentDepth, shouldForceMatricesDirty, shouldForceAlphaDirty);

			var layer = Futilef.Rendering.FRenderer.GetRenderLayer(_frame.atlas.texture, _shader, Futilef.Rendering.FPrimitiveType.Quad);
			int index = layer.GetQuota(1);

			var vertices = layer.vertices;
			var uvs = layer.uvs;
			var colors = layer.colors;

			int vertexStart = layer.PrimitiveIndexToVertexIndex(index);
			_concatenatedMatrix.ApplyTransform3D(ref vertices[vertexStart + 0], _frame.rectLeftBottom);
			_concatenatedMatrix.ApplyTransform3D(ref vertices[vertexStart + 1], _frame.rectLeftTop);
			_concatenatedMatrix.ApplyTransform3D(ref vertices[vertexStart + 2], _frame.rectRightTop);
			_concatenatedMatrix.ApplyTransform3D(ref vertices[vertexStart + 3], _frame.rectRightBottom);

			uvs[vertexStart + 0] = _frame.uvLeftBottom;
			uvs[vertexStart + 1] = _frame.uvLeftTop;
			uvs[vertexStart + 2] = _frame.uvRightTop;
			uvs[vertexStart + 3] = _frame.uvRightBottom;

			var color = new Color32(255, 255, 255, (byte)(255f * _concatenatedAlpha));
			colors[vertexStart + 0] = color;
			colors[vertexStart + 1] = color;
			colors[vertexStart + 2] = color;
			colors[vertexStart + 3] = color;
		}
	}
}

