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
			public Quad uvQuad;
			public Rect border;

			public Sprite(string[] segs, ref int i, float width, float height) {
				name = segs[i++];
				rotated = segs[i++] == "1";
				size = new Vec2(int.Parse(segs[i++]), int.Parse(segs[i++]));
				pivot = new Vec2(int.Parse(segs[i++]), int.Parse(segs[i++]));
				rect = new Rect(int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]));
				var uvRect = new Rect(int.Parse(segs[i++]) / width, int.Parse(segs[i++]) / height, int.Parse(segs[i++]) / width, int.Parse(segs[i++]) / height);
				uvRect.y = 1 - (uvRect.y + uvRect.h);
				uvQuad = new Quad(uvRect);
				if (rotated) {
					var bl = uvQuad.bl;
					uvQuad.bl = uvQuad.tl;
					uvQuad.tl = uvQuad.tr;
					uvQuad.tr = uvQuad.br;
					uvQuad.br = bl;
				}
				border = new Rect(int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]), int.Parse(segs[i++]));
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

