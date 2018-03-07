using System.Collections.Generic;

using UnityEngine;

namespace Futilef {
	public class FShader {
		public static FShader Default, Basic, Additive, AdditiveColored, Solid, SolidColored;

		static readonly List<FShader> _shaders = new List<FShader>();

		public int index;
		public string name;
		public Shader shader;

		FShader(int index, string name, Shader shader) {
			this.index = index;
			this.name = name;
		
			this.shader = shader;
		}

		public static void Init() {
			Default = Basic = Create("Basic", Shader.Find("Futile/Basic"));	
			Additive = Create("Additive", Shader.Find("Futile/Additive"));	
			AdditiveColored = Create("AdditiveColor", Shader.Find("Futile/AdditiveColor"));	
			Solid = Create("Solid", Shader.Find("Futile/Solid"));	
			SolidColored = Create("SolidColored", Shader.Find("Futile/SolidColored"));	
		}

		public static FShader Create(string name, Shader shader) {
			foreach (var fs in _shaders) if (fs.name.Equals(name)) return fs;

			var fShader = new FShader(_shaders.Count, name, shader);
			_shaders.Add(fShader);
			return fShader;
		}
	}
}

