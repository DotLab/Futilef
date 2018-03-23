using System.Collections.Generic;

using Futilef.Serialization;

using UnityEngine;

namespace Futilef.Node.Display {
	public class FLabel : FNode {
		public readonly BmFont font;
		public readonly Shader shader;
		public string text;

		public FLabel(BmFont font, Shader shader) {
			this.font = font;
			this.shader = shader;
		}

		public override void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			base.Redraw(ref currentDepth, shouldForceMatricesDirty, shouldForceAlphaDirty);
					
			var layer = Futilef.Rendering.FRenderer.GetRenderLayer(font.atlas.texture, shader, Futilef.Rendering.FPrimitiveType.Quad);
			int index = layer.PrimitiveIndexToVertexIndex(layer.GetQuota(text.Length));

			var vertices = layer.vertices;
			var uvs = layer.uvs;
			var colors = layer.colors;

			float currentX = 0, currentY = 0;

			for (int i = 0; i < text.Length; i++) {
				if (!font.HasGlyph(text[i])) continue;

				if (i > 0) currentX += font.GetKerning(text[i - 1], text[i]);

				var glyph = font.GetGlyph(text[i]);

				float posX = currentX + glyph.xOffset;
				float posY = currentY - glyph.yOffset;

				float rectLeft = posX;
				float rectRight = posX + glyph.width;
				float rectTop = posY;
				float rectBottom = posY - glyph.height;

				int j = index + (i << 2);
				vertices[j + 0].Set(rectLeft, rectBottom, 0);
				vertices[j + 1].Set(rectLeft, rectTop, 0);
				vertices[j + 2].Set(rectRight, rectTop, 0);
				vertices[j + 3].Set(rectRight, rectBottom, 0);

				uvs[j + 0] = glyph.uvLeftBottom;
				uvs[j + 1] = glyph.uvLeftTop;
				uvs[j + 2] = glyph.uvRightTop;
				uvs[j + 3] = glyph.uvRightBottom;

				colors[j + 0] = new Color32(255, 255, 255, 255);
				colors[j + 1] = new Color32(255, 255, 255, 255);
				colors[j + 2] = new Color32(255, 255, 255, 255);
				colors[j + 3] = new Color32(255, 255, 255, 255);

				currentX += glyph.xAdvance;
			}
		}
	}
}

