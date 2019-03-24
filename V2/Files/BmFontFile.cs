using System.Collections.Generic;

namespace Futilef.V2 {
	[System.Serializable]
	public sealed class BmFontFile {
		public struct Glyph {
			// id			4	uint	0+c*20	These fields are repeated until all characters have been described
			public uint id;
			// x			2	uint	4+c*20
			public ushort x;
			// y			2	uint	6+c*20
			public ushort y;
			// width		2	uint	8+c*20
			public ushort width;
			// height		2	uint	10+c*20
			public ushort height;
			// xoffset		2	int		12+c*20
			public short xOffset;
			// yoffset		2	int		14+c*20
			public short yOffset;
			// xadvance		2	int		16+c*20
			public short xAdvance;
			// page			1	uint	18+c*20
			public byte page;
			// chnl			1	uint	19+c*20

			public Rect uvRect;

			public Glyph(byte[] bytes, ref int i, float scaleW, float scaleH) {
				// id			4	uint	0+c*20	These fields are repeated until all characters have been described
				id = Bit.ReadUInt32(bytes, ref i);
				// x			2	uint	4+c*20
				x = Bit.ReadUInt16(bytes, ref i);
				// y			2	uint	6+c*20
				y = Bit.ReadUInt16(bytes, ref i);
				// width		2	uint	8+c*20
				width = Bit.ReadUInt16(bytes, ref i);
				// height		2	uint	10+c*20
				height = Bit.ReadUInt16(bytes, ref i);
				// xoffset		2	int		12+c*20
				// yoffset		2	int		14+c*20
				xOffset = Bit.ReadInt16(bytes, ref i);
				yOffset = Bit.ReadInt16(bytes, ref i);
				// xadvance		2	int		16+c*20
				xAdvance = Bit.ReadInt16(bytes, ref i);
				// page			1	uint	18+c*20
				page = Bit.ReadUInt8(bytes, ref i);
				// chnl			1	uint	19+c*20
				Bit.ReadUInt8(bytes, ref i);

				uvRect = new Rect(x / scaleW, 1f - (y + height) / scaleH, width / scaleW, height / scaleH);
			}
		}

		public struct LineDrawInfo {
			public Rect size;
			public Rect meshSize;
			public CharDrawInfo[] charDrawInfos;
		}

		public struct CharDrawInfo {
			public int page;
			public Rect rect;
			public Rect uvRect;
		}

		// fontSize		2	int		0
		public short fontSize;
		// bitField		1	bits	2	bit 0: smooth, bit 1: unicode, bit 2: italic, bit 3: bold, bit 4: fixedHeigth, bits 5-7: reserved
		// charSet		1	uint	3
		// stretchH		2	uint	4
		// aa			1	uint	6
		// paddingUp	1	uint	7
		// paddingRight	1	uint	8
		// paddingDown	1	uint	9
		// paddingLeft	1	uint	10
		// spacingHoriz	1	uint	11
		// spacingVert	1	uint	12
		// outline		1	uint	13	added with version 2
		// fontName		n+1	string	14	null terminated string with length n
		public string fontName;

		// lineHeight	2	uint	0
		public ushort lineHeight;
		// base			2	uint	2
		public ushort lineBase;
		// scaleW		2	uint	4
		public ushort scaleW;
		// scaleH		2	uint	6
		public ushort scaleH;
		// pages		2	uint	8
		public ushort pages;
		// bitField		1	bits	10	bits 0-6: reserved, bit 7: packed
		// alphaChnl	1	uint	11
		// redChnl		1	uint	12
		// greenChnl	1	uint	13
		// blueChnl		1	uint	14

		// pageNames	p*(n+1)	strings	0	p null terminated strings, each with length n
		public string[] pageNames;

		public readonly Dictionary<uint, Glyph> glyphDict = new Dictionary<uint, Glyph>();
		public readonly Dictionary<ulong, short> kerningDict = new Dictionary<ulong, short>();

		public BmFontFile(byte[] bytes) {
			int i = 0;

			// 'B'
			Bit.ReadUInt8(bytes, ref i);
			// 'M'
			Bit.ReadUInt8(bytes, ref i);
			// 'F'
			Bit.ReadUInt8(bytes, ref i);
			// 3
			Bit.ReadUInt8(bytes, ref i);

			// 1
			Bit.ReadUInt8(bytes, ref i);
			int length = Bit.ReadInt32(bytes, ref i);

			// fontSize		2	int		0
			fontSize = Bit.ReadInt16(bytes, ref i);
			// bitField		1	bits	2	bit 0: smooth, bit 1: unicode, bit 2: italic, bit 3: bold, bit 4: fixedHeigth, bits 5-7: reserved
			Bit.ReadUInt8(bytes, ref i);
			// charSet		1	uint	3
			Bit.ReadUInt8(bytes, ref i);
			// stretchH		2	uint	4
			Bit.ReadUInt16(bytes, ref i);
			// aa			1	uint	6
			Bit.ReadUInt8(bytes, ref i);
			// paddingUp	1	uint	7
			Bit.ReadUInt8(bytes, ref i);
			// paddingRight	1	uint	8
			Bit.ReadUInt8(bytes, ref i);
			// paddingDown	1	uint	9
			Bit.ReadUInt8(bytes, ref i);
			// paddingLeft	1	uint	10
			Bit.ReadUInt8(bytes, ref i);
			// spacingHoriz	1	uint	11
			Bit.ReadUInt8(bytes, ref i);
			// spacingVert	1	uint	12
			Bit.ReadUInt8(bytes, ref i);
			// outline		1	uint	13	added with version 2
			Bit.ReadUInt8(bytes, ref i);
			// fontName		n+1	string	14	null terminated string with length n
			fontName = Bit.ReadString(bytes, ref i, length - 15);
			// '\0'
			Bit.ReadUInt8(bytes, ref i);

//			UnityEngine.Debug.Log(fontName);

			// 2
			Bit.ReadUInt8(bytes, ref i);
			length = Bit.ReadInt32(bytes, ref i);

			// lineHeight	2	uint	0
			lineHeight = Bit.ReadUInt16(bytes, ref i);
			// base			2	uint	2
			lineBase = Bit.ReadUInt16(bytes, ref i);
			// scaleW		2	uint	4
			scaleW = Bit.ReadUInt16(bytes, ref i);
			// scaleH		2	uint	6
			scaleH = Bit.ReadUInt16(bytes, ref i);
			// pages		2	uint	8
			pages = Bit.ReadUInt16(bytes, ref i);
			// bitField		1	bits	10	bits 0-6: reserved, bit 7: packed
			Bit.ReadUInt8(bytes, ref i);
			// alphaChnl	1	uint	11
			Bit.ReadUInt8(bytes, ref i);
			// redChnl		1	uint	12
			Bit.ReadUInt8(bytes, ref i);
			// greenChnl	1	uint	13
			Bit.ReadUInt8(bytes, ref i);
			// blueChnl		1	uint	14
			Bit.ReadUInt8(bytes, ref i);

			// 3
			Bit.ReadUInt8(bytes, ref i);
			length = Bit.ReadInt32(bytes, ref i);

			pageNames = new string[pages];
			for (int j = 0; j < pages; j += 1) {
				pageNames[j] = Bit.TrimNull(Bit.ReadString(bytes, ref i, length / pages));
//				UnityEngine.Debug.Log(pageNames[j]);
			}

			// 4
			Bit.ReadUInt8(bytes, ref i);
			length = Bit.ReadInt32(bytes, ref i);

			for (int j = 0; j < length; j += 20) {
				var glyph = new Glyph(bytes, ref i, scaleW, scaleH);
//				UnityEngine.Debug.LogFormat("{0:x}", glyph.id);
				glyphDict.Add(glyph.id, glyph);
			}
//			UnityEngine.Debug.Log(glyphDict.Count);

			if (i >= bytes.Length) return;

			// 5
			Bit.ReadUInt8(bytes, ref i);
			length = Bit.ReadInt32(bytes, ref i);

			for (int j = 0; j < length; j += 10) {
				uint first = Bit.ReadUInt32(bytes, ref i);
				uint second = Bit.ReadUInt32(bytes, ref i);
				short amount = Bit.ReadInt16(bytes, ref i);

				kerningDict.Add(((ulong)first << 32) | (ulong)second, amount);

//				UnityEngine.Debug.LogFormat("{0}-{1}: {2}", first, second, amount);
			}
		}

		public short GetKerning(uint first, uint second) {
			short k;
			kerningDict.TryGetValue(((ulong)first << 32) | (ulong)second, out k);
			return k;
		}
	
		public LineDrawInfo GenerateDrawInfo(string line) {
			int j = 0;
			int curX = 0;
			uint lastChar = 0;

			var infoList = new List<CharDrawInfo>();
			float meshLeft = 0;
			float meshRight = 0;
			float meshTop = 0;
			float meshBottom = 0;

			for (int i = 0, end = line.Length; i < end; i++) {
				Glyph g;
				char c = line[i];
				if (!glyphDict.TryGetValue(c, out g)) continue;

				if (char.IsWhiteSpace(c)) {
					curX += g.xAdvance;
					continue;
				}

				if (j > 0) curX += GetKerning(lastChar, c);

				float left = curX + g.xOffset;
				float right = left + g.width;
				float top = lineBase - g.yOffset;
				float bottom = top - g.height;

				if (j == 0) {
					meshLeft = left;
					meshRight = right;
					meshTop = top;
					meshBottom = bottom;
				} else {
					if (left < meshLeft) meshLeft = left;
					if (right > meshRight) meshRight = right;
					if (top > meshTop) meshTop = top;
					if (bottom < meshBottom) meshBottom = bottom;
				}

				infoList.Add(new CharDrawInfo{
					page = g.page,
					uvRect = g.uvRect,
					rect = new Rect(left, bottom, g.width, g.height),
				});

				curX += g.xAdvance;
				lastChar = c;
				j += 1;
			}

			return new LineDrawInfo{ 
				size = new Rect(0, lineBase - lineHeight, curX, lineBase),
				meshSize = new Rect(meshLeft, meshBottom, meshRight - meshLeft, meshTop - meshBottom),
				charDrawInfos = infoList.ToArray(), 
			};
		}
	}
}