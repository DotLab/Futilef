using System;
using System.Collections.Generic;

using UnityEngine;


namespace Futilef {
	public static class FAtlasManager {
		static readonly List<FAtlas> _atlases = new List<FAtlas>();
		static readonly Dictionary<string, FAtlas> _atlasMapping = new Dictionary<string, FAtlas>();
		static readonly Dictionary<string, FAtlasElement> _elementsMapping = new Dictionary<string, FAtlasElement>();

		static readonly List<FFont> _fonts = new List<FFont>();
		static readonly Dictionary<string, FFont> _fontMapping = new Dictionary<string, FFont>();

		public static void Init() {
		}

		#region Atlas

		public static FAtlasElement GetElement(string name) {
			if (_elementsMapping.ContainsKey(name)) return _elementsMapping[name];
			throw new Exception("Cannot find element");
		}

		public static FAtlas LoadAtlas(string name, Texture texture) {
			var atlas = new FAtlas(_atlases.Count, name, texture);
			return AddAtlas(atlas);
		}

		public static FAtlas LoadAtlas(string name) {
			var atlas = new FAtlas(_atlases.Count, name, name, name);
			return AddAtlas(atlas);
		}

		public static FAtlas LoadImage(string name) {
			var atlas = new FAtlas(_atlases.Count, name, name);
			return AddAtlas(atlas);
		}

		static FAtlas AddAtlas(FAtlas atlas) {
			if (_atlasMapping.ContainsKey(atlas.name)) throw new Exception("Duplicate atlas"); 

			foreach (var element in atlas.elements) {
				if (_elementsMapping.ContainsKey(element.name)) throw new Exception("Duplicate element");
				_elementsMapping.Add(element.name, element);
			}
			_atlases.Add(atlas);
			_atlasMapping.Add(atlas.name, atlas);

			return atlas;
		}

		#endregion

		#region Font

		public static FFont GetFont(string name) {
			if (_fontMapping.ContainsKey(name)) return _fontMapping[name];
			throw new Exception("Cannot find font");
		}

		public static void LoadFont(string name, string elementName, string configPath, float offsetX, float offsetY, FTextParams textParams) {
			var element = GetElement(elementName);
			var font = new FFont(name, element, configPath, offsetX, offsetY, textParams);
			_fonts.Add(font);
			_fontMapping.Add(name, font);
		}

		#endregion

	}
}

