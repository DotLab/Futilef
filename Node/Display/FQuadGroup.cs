using UnityEngine;

using Futilef.Serialization;

namespace Futilef.Node.Display {
	public class FQuadGroup : FNode {
		public TpFrame frame;
		public Shader shader;

		public Vector2[] localVertices;
		public Color32[] quadColors;
		
		public FQuadGroup(TpFrame frame, Shader shader) {
			this.frame = frame;
			this.shader = shader;
		}

		public override void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			if (localVertices == null || quadColors == null || localVertices.Length != (quadColors.Length << 2)) return;

			base.Redraw(ref currentDepth, shouldForceMatricesDirty, shouldForceAlphaDirty);

			var layer = Futilef.Rendering.FRenderer.GetRenderLayer(frame.atlas.texture, shader, Futilef.Rendering.FPrimitiveType.Quad);
			int index = layer.PrimitiveIndexToVertexIndex(layer.GetQuota(1));

			var vertices = layer.vertices;
			var uvs = layer.uvs;
			var colors = layer.colors;

			for (int i = 0; i < quadColors.Length; i++) {
				int j = index + (i << 2), k = (i << 2);

				_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 0], localVertices[k + 0]);
				_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 1], localVertices[k + 1]);
				_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 2], localVertices[k + 2]);
				_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 3], localVertices[k + 3]);
				
				uvs[j + 0] = frame.uvLeftBottom;
				uvs[j + 1] = frame.uvLeftTop;
				uvs[j + 2] = frame.uvRightTop;
				uvs[j + 3] = frame.uvRightBottom;

				var color = quadColors[i];
				var vertexColor = new Color32(color.r, color.g, color.b, (byte)(_concatenatedAlpha * color.a));
				colors[index + 0] = vertexColor;
				colors[index + 1] = vertexColor;
				colors[index + 2] = vertexColor;
				colors[index + 3] = vertexColor;
			}
		}
	}
}

