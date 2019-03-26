using System.Collections.Generic;

namespace Futilef.V2 {
	[System.Serializable]
	public class TpDataFile {
		[System.Serializable]
		public struct Sprite {
			public string name;
			public bool rotated;
			public Vec2 size;
			public Vec2 pivot;
			public Rect rect;
			public Rect rectInner;
			public Rect uv;

			public Border border;
			public Quad uvQuad;
			public Quad uvQuadInner;

			public Sprite(string[] segs, ref int i, float width, float height) {
				name = segs[i++];

				rotated = segs[i++] == "1";

				size = new Vec2(int.Parse(segs[i++]), int.Parse(segs[i++]));
				pivot = new Vec2(int.Parse(segs[i++]), int.Parse(segs[i++]));

				rect = new Rect(int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]));
				rect.y = size.y - (rect.y + rect.h);

				rectInner = new Rect(int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]));
				rectInner.y = size.y - (rectInner.y + rectInner.h);

				border = new Border(rectInner.x, size.x - (rectInner.x + rectInner.w), rectInner.y, size.y - (rectInner.y + rectInner.h));

				uv = new Rect(int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]));
				uv.y = height - (uv.y + uv.h);

				uvQuad = new Quad(uv.x / width, uv.y / height, uv.w / width, uv.h / height);
				uvQuadInner = new Quad((uv.x + rectInner.x) / width, (uv.y + rectInner.y) / height, rectInner.w / width, rectInner.h / height);

				if (rotated) {
					var bl = uvQuad.bl;
					uvQuad.bl = uvQuad.tl;
					uvQuad.tl = uvQuad.tr;
					uvQuad.tr = uvQuad.br;
					uvQuad.br = bl;

					bl = uvQuadInner.bl;
					uvQuadInner.bl = uvQuad.tl;
					uvQuadInner.tl = uvQuad.tr;
					uvQuadInner.tr = uvQuad.br;
					uvQuadInner.br = bl;
				}
			}
		}

		public string name;

		public int width;
		public int height;

		public int spriteCount;
//		public Sprite[] sprites;
		public readonly Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

		public TpDataFile(string str) {
			int i = 0;

			string[] segs = str.Split(',');

			name = segs[i++];
			width = int.Parse(segs[i++]);
			height = int.Parse(segs[i++]);

			spriteCount = int.Parse(segs[i++]);
//			sprites = new Sprite[spriteCount];

			for (int j = 0; j < spriteCount; j += 1) {
				var sprite = new Sprite(segs, ref i, width, height);
				spriteDict.Add(sprite.name, sprite);
			}
		}
	}
}

