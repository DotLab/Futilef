using System.Collections.Generic;
using UnityEngine;

namespace Futilef {
	public static unsafe class Res {
		static readonly Dictionary<int, Texture2D> textureDict = new Dictionary<int, Texture2D>();

		static readonly PtrLst *atlasMetaLst = PtrLst.New();
		static readonly PtrLst *spriteMetaLst = PtrLst.New();
		static readonly Dictionary<int, int> spriteMetaLstIdxDict = new Dictionary<int, int>();

		public static Texture2D GetTexture(int id) {
			if (textureDict.ContainsKey(id)) return textureDict[id];

			var texture = new Texture2D(0, 0);
//			texture.wrapMode = TextureWrapMode.Clamp;
//			Debug.Log("Load " + id + "i");
			texture.LoadImage(Resources.Load<TextAsset>(id + "i").bytes);
			textureDict.Add(id, texture);
			return texture;
		}

		public static void LoadAtlases(params int[] ids) {
			for (int i = 0, len = ids.Length; i < len; i += 1) {
				int id = ids[i];
				var atlasMeta = TpAtlasMeta.New(Resources.Load<TextAsset>(id.ToString()).text);
				PtrLst.Push(atlasMetaLst, atlasMeta);
				for (int j = 0, jlen = atlasMeta->spriteCount; j < jlen; j += 1) {
					var spriteMeta = atlasMeta->sprites + j;
					spriteMetaLstIdxDict.Add(spriteMeta->name, spriteMetaLst->count);
					PtrLst.Push(spriteMetaLst, spriteMeta);
				}
			}	
		}

		public static TpSpriteMeta *GetSpriteMeta(int id) {
			return (TpSpriteMeta *)spriteMetaLst->arr[spriteMetaLstIdxDict[id]];
		}

		public static bool HasSpriteMeta(int id) {
			return spriteMetaLstIdxDict.ContainsKey(id);
		}
	}
}
