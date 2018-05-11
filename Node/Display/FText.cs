using System.Collections.Generic;

using Futilef.Serialization;

using UnityEngine;

namespace Futilef.Node.Display {
	public class FText : FNode {
		public readonly BmFont font;
		public readonly Shader shader;

		string _text = "";
		public string text {
			get { return _text; }
			set {
				if (value == _text) return;

				_isTextDirty = true;
				_text = value;
			}
		}

		FVerticalAlignment _verticalAlignment = FVerticalAlignment.Top;
		public FVerticalAlignment verticalAlignment {
			get { return _verticalAlignment; }
			set {
				_isTextDirty = true;
				_verticalAlignment = value;
			}
		}

		FHorizontalAlignment _horizontalAlignment = FHorizontalAlignment.Left;
		public FHorizontalAlignment horizontalAlignment {
			get { return _horizontalAlignment; }
			set {
				_isTextDirty = true;
				_horizontalAlignment = value;
			}
		}

		bool _isTextDirty;

		BmGlyph[] _glyphs = new BmGlyph[1];
		Vector2[] _localVertices = new Vector2[4];
		int[] _widths = new int[1];

		public FText(BmFont font, Shader shader) {
			this.font = font;
			this.shader = shader;
		}

		public override void Redraw(ref int currentDepth, bool shouldForceMatricesDirty, bool shouldForceAlphaDirty) {
			base.Redraw(ref currentDepth, shouldForceMatricesDirty, shouldForceAlphaDirty);

			if (_isTextDirty) RecalculateLocalVertices();

			var layer = Futilef.Rendering.FRenderer.GetRenderLayer(font.atlas.texture, shader, Futilef.Rendering.FPrimitiveType.Quad);
			int index = layer.PrimitiveIndexToVertexIndex(layer.GetQuota(_text.Length));

			var vertices = layer.vertices;
			var uvs = layer.uvs;
			var colors = layer.colors;

			for (int i = 0; i < text.Length; i++) {
				var glyph = _glyphs[i];
				if (glyph == null) continue;

				int j = index + (i << 2), k = (i << 2);
				_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 0], _localVertices[k + 0]);
				_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 1], _localVertices[k + 1]);
				_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 2], _localVertices[k + 2]);
				_concatenatedMatrix.ApplyTransform3D(ref vertices[j + 3], _localVertices[k + 3]);

				uvs[j + 0] = glyph.uvLeftBottom;
				uvs[j + 1] = glyph.uvLeftTop;
				uvs[j + 2] = glyph.uvRightTop;
				uvs[j + 3] = glyph.uvRightBottom;

				colors[j + 0] = new Color32(255, 255, 255, 255);
				colors[j + 1] = new Color32(255, 255, 255, 255);
				colors[j + 2] = new Color32(255, 255, 255, 255);
				colors[j + 3] = new Color32(255, 255, 255, 255);
			}
		}

		void RecalculateLocalVertices() {
			_isTextDirty = false;

			if (_text.Length > _glyphs.Length) {
				_glyphs = new BmGlyph[System.Math.Max(_text.Length, _glyphs.Length << 1)];
				_localVertices = new Vector2[_glyphs.Length << 2];
			}

			int currentLine = 0;
			float currentX = 0, currentY = 0;
			for (int i = 0, lineX = 0; i < _text.Length; i++) {
				if (_text[i] == '\n') {
					_widths[currentLine] = lineX;

					lineX = 0;
					currentLine += 1;
					if (currentLine >= _widths.Length) System.Array.Resize(ref _widths, _widths.Length << 1);
				} else {
					if (!font.TryGetGlyph(_text[i], out _glyphs[i])) continue;
					if (i > 0) lineX += font.GetKerning(_text[i - 1], _text[i]);
					lineX += _glyphs[i].xAdvance;
				}
			}

			if (_verticalAlignment != FVerticalAlignment.Top) {
				currentY = _verticalAlignment == FVerticalAlignment.Middle ? font.lineHeight * (currentLine + 1) >> 1 : font.lineHeight * (currentLine + 1);
			}

			if (_horizontalAlignment != FHorizontalAlignment.Left) {
				currentX = _horizontalAlignment == FHorizontalAlignment.Center ? _widths[0] >> 1 : _widths[0];
			}

			for (int i = 0, k = 1; i < _text.Length; i++) {
				if (_text[i] == '\n') {
					if (_horizontalAlignment != FHorizontalAlignment.Left) {
						currentX = _horizontalAlignment == FHorizontalAlignment.Center ? _widths[k] >> 1 : _widths[k];
					} else currentX = 0;
					currentY -= font.lineHeight;
					continue;
				}

				var glyph = _glyphs[i];
				if (glyph == null) continue;

				if (i > 0) currentX += font.GetKerning(_text[i - 1], _text[i]);

				float posX = currentX + glyph.xOffset;
				float posY = currentY - glyph.yOffset;

				float rectLeft = posX;
				float rectRight = posX + glyph.width;
				float rectTop = posY;
				float rectBottom = posY - glyph.height;

				int j = (i << 2);

				_localVertices[j + 0].Set(rectLeft, rectBottom);
				_localVertices[j + 1].Set(rectLeft, rectTop);
				_localVertices[j + 2].Set(rectRight, rectTop);
				_localVertices[j + 3].Set(rectRight, rectBottom);

				currentX += glyph.xAdvance;
				_glyphs[i] = glyph;
			}
		}
	}
}

