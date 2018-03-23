using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;

namespace Futilef.Serialization {
	#region Streamable
	public abstract class Streamable {
		protected static class Assert {
			public static void Equal(Int32 a, Int32 b) {
				if (a != b) throw new InvalidProgramException(string.Format("{0} != {1}", a, b));
			}

			public static void Equal(UInt32 a, UInt32 b) {
				if (a != b) throw new InvalidProgramException(string.Format("{0} != {1}", a, b));
			}
		}

		protected readonly Stream _stream;

		protected Streamable(Stream stream) {
			_stream = stream;
		}

		protected bool IsEnd() {
			return _stream.Position >= _stream.Length;
		}

		protected void Skip(int val) {
			_stream.Position += val;
		}

		protected Char ReadChar() {
			return (char)_stream.ReadByte();
		}

		protected Byte ReadByte() {
			return (byte)_stream.ReadByte();
		}

		protected string ReadString(int count) {
			var buffer = new byte[count];
			_stream.Read(buffer, 0, count);
			return System.Text.Encoding.UTF8.GetString(buffer).Trim();
		}

		protected Int16 ReadInt16() {
			return (short)(_stream.ReadByte() | (sbyte)_stream.ReadByte() << 8);
		}

		protected Int32 ReadInt32() {
			return _stream.ReadByte() | _stream.ReadByte() << 8 | _stream.ReadByte() << 16 | (sbyte)_stream.ReadByte() << 24;
		}

		protected UInt16 ReadUInt16() {
			return (ushort)(_stream.ReadByte() | _stream.ReadByte() << 8);
		}

		protected UInt32 ReadUInt32() {
			return (uint)(_stream.ReadByte() | _stream.ReadByte() << 8 | _stream.ReadByte() << 16 | _stream.ReadByte() << 24);
		}
	}
	#endregion

	public class BmGlyph : Streamable {
		// id			4	uint	0+c*20	These fields are repeated until all characters have been described
		public readonly UInt32 id;
		// x			2	uint	4+c*20
		// y			2	uint	6+c*20
		public readonly UInt32 x, y;
		// width		2	uint	8+c*20
		// height		2	uint	10+c*20
		public readonly UInt32 width, height;
		// xoffset		2	int		12+c*20
		// yoffset		2	int		14+c*20
		public readonly Int32 xOffset, yOffset;
		// xadvance		2	int		16+c*20
		public readonly Int32 xAdvance;
		// page			1	uint	18+c*20
		public readonly UInt32 page;
		// chnl			1	uint	19+c*20

		// Non-serialized
		public Vector2 uvLeftBottom, uvLeftTop, uvRightTop, uvRightBottom;

		public BmGlyph(Stream stream) : base(stream) {
			// id			4	uint	0+c*20	These fields are repeated until all characters have been described
			id = ReadUInt32();
			// x			2	uint	4+c*20
			// y			2	uint	6+c*20
			x = ReadUInt16();
			y = ReadUInt16();
			// width		2	uint	8+c*20
			// height		2	uint	10+c*20
			width = ReadUInt16();
			height = ReadUInt16();
			// xoffset		2	int		12+c*20
			// yoffset		2	int		14+c*20
			xOffset = ReadInt16();
			yOffset = ReadInt16();
			// xadvance		2	int		16+c*20
			xAdvance = ReadInt16();
			// page			1	uint	18+c*20
			page = ReadByte();
			// chnl			1	uint	19+c*20
			ReadByte();

			Debug.Log(this);
		}

		public override string ToString() {
			return string.Format("[BmGlyph: id={0}, x={1}, y={2}, width={3}, height={4}, xOffset={5}, yOffset={6}, xAdvance={7}, page={8}]", id, x, y, width, height, xOffset, yOffset, xAdvance, page);
		}
	}

	public class BmFont : Streamable {
		// fontSize		2	int		0
		public readonly Int16 fontSize;
		// bitField		1	bits	2	bit 0: smooth, bit 1: unicode, bit 2: italic, bit 3: bold, bit 4: fixedHeigth, bits 5-7: reserved
		// charSet		1	uint	3
		// stretchH		2	uint	4
		// aa			1	uint	6
		// paddingUp	1	uint	7
		// paddingRight	1	uint	8
		// paddingDown	1	uint	9
		// paddingLeft	1	uint	10
		// spacingHoriz	1	uint	11
		public readonly byte spacingHoriz;
		// spacingVert	1	uint	12
		public readonly byte spacingVert;
		// outline		1	uint	13	added with version 2
		// fontName		n+1	string	14	null terminated string with length n
		public readonly string fontName;

		// lineHeight	2	uint	0
		public readonly UInt16 lineHeight;
		// base			2	uint	2
		// scaleW		2	uint	4
		// scaleH		2	uint	6
		public readonly UInt16 scaleW, scaleH;
		// pages		2	uint	8
		public readonly UInt16 pages;
		// bitField		1	bits	10	bits 0-6: reserved, bit 7: packed
		// alphaChnl	1	uint	11
		// redChnl		1	uint	12
		// greenChnl	1	uint	13
		// blueChnl		1	uint	14

		// pageNames	p*(n+1)	strings	0	p null terminated strings, each with length n
		public readonly string[] pageNames;

		// Non-serialized
		public TpAtlas atlas;

		public readonly Dictionary<UInt32, BmGlyph> glyphDict = new Dictionary<UInt32, BmGlyph>();
		public readonly Dictionary<UInt64, Int16> kerningDict = new Dictionary<UInt64, Int16>();

		public BmFont(Stream stream) : base(stream) {
			Assert.Equal('B', ReadChar());
			Assert.Equal('M', ReadChar());
			Assert.Equal('F', ReadChar());
			Assert.Equal(3, ReadByte());

			Assert.Equal(1, ReadByte());
			var length = ReadInt32();

			// fontSize		2	int		0
			fontSize = ReadInt16();
			// bitField		1	bits	2	bit 0: smooth, bit 1: unicode, bit 2: italic, bit 3: bold, bit 4: fixedHeigth, bits 5-7: reserved
			ReadByte();
			// charSet		1	uint	3
			ReadByte();
			// stretchH		2	uint	4
			ReadUInt16();
			// aa			1	uint	6
			ReadByte();
			// paddingUp	1	uint	7
			ReadByte();
			// paddingRight	1	uint	8
			ReadByte();
			// paddingDown	1	uint	9
			ReadByte();
			// paddingLeft	1	uint	10
			ReadByte();
			// spacingHoriz	1	uint	11
			spacingHoriz = ReadByte();
			// spacingVert	1	uint	12
			spacingVert = ReadByte();
			// outline		1	uint	13	added with version 2
			ReadByte();
			// fontName		n+1	string	14	null terminated string with length n
			fontName = ReadString(length - 15);
			Assert.Equal('\0', ReadChar());

			Debug.Log(fontName);

			Assert.Equal(2, ReadByte());
			length = ReadInt32();

			// lineHeight	2	uint	0
			lineHeight = ReadUInt16();
			// base			2	uint	2
			ReadUInt16();
			// scaleW		2	uint	4
			// scaleH		2	uint	6
			scaleW = ReadUInt16();
			scaleH = ReadUInt16();
			// pages		2	uint	8
			pages = ReadUInt16();
			// bitField		1	bits	10	bits 0-6: reserved, bit 7: packed
			ReadByte();
			// alphaChnl	1	uint	11
			ReadByte();
			// redChnl		1	uint	12
			ReadByte();
			// greenChnl	1	uint	13
			ReadByte();
			// blueChnl		1	uint	14
			ReadByte();

			Assert.Equal(3, ReadByte());
			length = ReadInt32();
			pageNames = ReadString(length).Split(new [] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
			Assert.Equal(pages, pageNames.Length);
			foreach (var n in pageNames) {
				Debug.Log(n);
			}

			Assert.Equal(4, ReadByte());
			length = ReadInt32();
			for (int i = 0; i < length; i += 20) {
				var glyph = new BmGlyph(stream);
				glyphDict.Add(glyph.id, glyph);
			}
			Debug.Log(glyphDict.Count);

			if (IsEnd()) return;

			Assert.Equal(5, ReadByte());
			length = ReadInt32();
			for (int i = 0; i < length; i += 10) {
				UInt64 first = ReadUInt32();
				UInt64 second = ReadUInt32();
				Int16 amount = ReadInt16();

				UInt64 key = (first << 32) | second;
				kerningDict.Add(key, amount);

				Debug.LogFormat("{0}-{1}: {2}", first, second, amount);
			}
		}

		public bool HasGlyph(UInt32 id) {
			return glyphDict.ContainsKey(id);
		}

		public BmGlyph GetGlyph(UInt32 id) {
			return glyphDict[id];
		}

		public int GetKerning(UInt32 first, UInt32 second) {
			UInt64 key = (first << 32) | second;
			return kerningDict.ContainsKey(key) ? kerningDict[key] : 0;
		}

		public override string ToString() {
			return string.Format("[BmFont: fontSize={0}, spacingHoriz={1}, spacingVert={2}, fontName={3}, lineHeight={4}, scaleW={5}, scaleH={6}, pages={7}, pageNames={8}, _glyphDict={9}, _kerningDict={10}]", fontSize, spacingHoriz, spacingVert, fontName, lineHeight, scaleW, scaleH, pages, pageNames, glyphDict, kerningDict);
		}
	}
}