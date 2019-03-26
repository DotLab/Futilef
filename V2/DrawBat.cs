using UnityEngine;
using Array = System.Array;

namespace Futilef.V2 {
	public sealed class DrawBat : System.IDisposable {
		const int VertExpandAmount = 60, TriExpandAmount = 30;
		const int VertUnusedLimit = 600, TriUnusedLimit = 300;

		public Shader shader;
		public Texture texture;

		readonly GameObject gameObject;

		readonly MeshFilter meshFilter;
		readonly Mesh mesh;

		readonly MeshRenderer meshRenderer;
		readonly Material material;

		public int vertLen = VertExpandAmount;
		public int vertCount;
		public Vector3[] verts = new Vector3[VertExpandAmount];
		public Color[] colors = new Color[VertExpandAmount];
		public Vector2[] uvs = new Vector2[VertExpandAmount];

		public int triLen = TriExpandAmount;
		public int triCount;
		public int[] tris = new int[TriExpandAmount];

		public DrawBat(Shader shader, Texture texture) {
			this.shader = shader; this.texture = texture;

			gameObject = new GameObject(string.Format("Batch [{0}] [{1}]", shader.name, texture.name));

			meshFilter = gameObject.AddComponent<MeshFilter>();
			mesh = meshFilter.mesh;
			mesh.MarkDynamic();

			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			material = new Material(shader.unityShader) { mainTexture = texture.unityTexture };
			meshRenderer.sharedMaterial = material;
		}

		public void Open(int queue) {
			verts[0].Set(50, 0, 1000000);  // special vertex to fill unused triangle
			vertCount = 1; triCount = 0;
			material.renderQueue = queue;
		}

		public void RequestQuota(int v, int t) {
			if ((vertCount += v) >= vertLen) {
				vertLen = vertCount + VertExpandAmount;
				Array.Resize(ref verts, vertLen);
				Array.Resize(ref colors, vertLen);
				Array.Resize(ref uvs, vertLen);
			}

			if ((triCount += t * 3) >= triLen) {
				triLen = triCount + TriExpandAmount;
				Array.Resize(ref tris, triLen);
			}
		}

		public void Close() {
			if (vertCount < vertLen - VertUnusedLimit) {
				vertLen = vertCount + VertExpandAmount;
				Array.Resize(ref verts, vertLen);
				Array.Resize(ref colors, vertLen);
				Array.Resize(ref uvs, vertLen);
			}

			if (triCount < triLen - TriUnusedLimit) {
				triLen = triCount + TriExpandAmount;
				Array.Resize(ref tris, triLen);
			}

			if (triCount < triLen - 1) {  // fill unused triangles
				Array.Clear(tris, triCount, triLen - triCount - 1);
			}

			mesh.vertices = verts;
			mesh.colors = colors;
			mesh.uv = uvs;

			mesh.triangles = tris;
		}

		public void Activate() {
//			gameObject.SetActive(true);
			meshRenderer.enabled = true;
		}

		public void Deactivate() {
//			gameObject.SetActive(false);
			meshRenderer.enabled = false;
		}

		public void Dispose() {
			if (gameObject != null) {
				Debug.Log("Dispose " + gameObject.name);
				Object.Destroy(material);
				Object.Destroy(mesh);
				Object.Destroy(gameObject);
			}
		}
	}

	public static class DrawBatExtension {
		/**
		 * 0 - 1
		 * | \ |
		 * 2 - 3
		 * 4, 2
		 */
		public static void DrawQuad(this DrawBat bat, Quad q, Quad uv, Vec4 c) {
			int vi = bat.vertCount;
			int ti = bat.triCount;
			bat.RequestQuota(4, 2);

			var verts = bat.verts;
			verts[vi + 0].Set(q.tl.x, q.tl.y, 0);
			verts[vi + 1].Set(q.tr.x, q.tr.y, 0);
			verts[vi + 2].Set(q.bl.x, q.bl.y, 0);
			verts[vi + 3].Set(q.br.x, q.br.y, 0);

			var uvs = bat.uvs;
			uvs[vi + 0].Set(uv.tl.x, uv.tl.y);
			uvs[vi + 1].Set(uv.tr.x, uv.tr.y);
			uvs[vi + 2].Set(uv.bl.x, uv.bl.y);
			uvs[vi + 3].Set(uv.br.x, uv.br.y);

			var color = new Color(c.x, c.y, c.z, c.w);
			var colors = bat.colors;
			colors[vi + 0] = color;
			colors[vi + 1] = color;
			colors[vi + 2] = color;
			colors[vi + 3] = color;

			var tris = bat.tris;
			tris[ti + 0] = vi;
			tris[ti + 1] = vi + 1;
			tris[ti + 2] = vi + 3;
			tris[ti + 3] = vi;
			tris[ti + 4] = vi + 3;
			tris[ti + 5] = vi + 2;
		}

		/**
		 * 0 -- 1 -- 2 -- 3
		 * | \  | \  | \  |
		 * 4 -- 5 -- 6 -- 7
		 * | \  | \  | \  |
		 * 8 -- 9 -- 10 - 11
		 * | \  | \  | \  |
		 * 12 - 13 - 14 - 15
		 * 16, 18
		 */
		public static void DrawQuad9Sliced(this DrawBat bat, Quad q, Quad qI, Quad uv, Quad uvI, Vec4 c, bool fillInner = true) {
			int vi = bat.vertCount;
			int ti = bat.triCount;
			bat.RequestQuota(16, fillInner ? 18 : 16);

			var verts = bat.verts;
			verts[vi + 0].Set(q.tl.x, q.tl.y, 0);
			verts[vi + 1].Set(qI.tl.x, q.tl.y, 0);
			verts[vi + 2].Set(qI.tr.x, q.tr.y, 0);
			verts[vi + 3].Set(q.tr.x, q.tr.y, 0);

			verts[vi + 4].Set(q.tl.x, qI.tl.y, 0);
			verts[vi + 5].Set(qI.tl.x, qI.tl.y, 0);
			verts[vi + 6].Set(qI.tr.x, qI.tr.y, 0);
			verts[vi + 7].Set(q.tr.x, qI.tr.y, 0);

			verts[vi + 8].Set(q.bl.x, qI.bl.y, 0);
			verts[vi + 9].Set(qI.bl.x, qI.bl.y, 0);
			verts[vi + 10].Set(qI.br.x, qI.br.y, 0);
			verts[vi + 11].Set(q.br.x, qI.br.y, 0);

			verts[vi + 12].Set(q.bl.x, q.bl.y, 0);
			verts[vi + 13].Set(qI.bl.x, q.bl.y, 0);
			verts[vi + 14].Set(qI.br.x, q.br.y, 0);
			verts[vi + 15].Set(q.br.x, q.br.y, 0);

			var uvs = bat.uvs;
			uvs[vi + 0].Set(uv.tl.x, uv.tl.y);
			uvs[vi + 1].Set(uvI.tl.x, uv.tl.y);
			uvs[vi + 2].Set(uvI.tr.x, uv.tr.y);
			uvs[vi + 3].Set(uv.tr.x, uv.tr.y);

			uvs[vi + 4].Set(uv.tl.x, uvI.tl.y);
			uvs[vi + 5].Set(uvI.tl.x, uvI.tl.y);
			uvs[vi + 6].Set(uvI.tr.x, uvI.tr.y);
			uvs[vi + 7].Set(uv.tr.x, uvI.tr.y);

			uvs[vi + 8].Set(uv.bl.x, uvI.bl.y);
			uvs[vi + 9].Set(uvI.bl.x, uvI.bl.y);
			uvs[vi + 10].Set(uvI.br.x, uvI.br.y);
			uvs[vi + 11].Set(uv.br.x, uvI.br.y);

			uvs[vi + 12].Set(uv.bl.x, uv.bl.y);
			uvs[vi + 13].Set(uvI.bl.x, uv.bl.y);
			uvs[vi + 14].Set(uvI.br.x, uv.br.y);
			uvs[vi + 15].Set(uv.br.x, uv.br.y);

			var color = new Color(c.x, c.y, c.z, c.w);
			var colors = bat.colors;
			colors[vi + 0] = color;
			colors[vi + 1] = color;
			colors[vi + 2] = color;
			colors[vi + 3] = color;
			colors[vi + 4] = color;
			colors[vi + 5] = color;
			colors[vi + 6] = color;
			colors[vi + 7] = color;
			colors[vi + 8] = color;
			colors[vi + 9] = color;
			colors[vi + 10] = color;
			colors[vi + 11] = color;
			colors[vi + 12] = color;
			colors[vi + 13] = color;
			colors[vi + 14] = color;
			colors[vi + 15] = color;

			var tris = bat.tris;
			tris[ti + 0] = vi;
			tris[ti + 1] = vi + 1;
			tris[ti + 2] = vi + 5;
			tris[ti + 3] = vi;
			tris[ti + 4] = vi + 5;
			tris[ti + 5] = vi + 4;

			tris[ti + 6] = vi + 1;
			tris[ti + 7] = vi + 2;
			tris[ti + 8] = vi + 6;
			tris[ti + 9] = vi + 1;
			tris[ti + 10] = vi + 6;
			tris[ti + 11] = vi + 5;

			tris[ti + 12] = vi + 2;
			tris[ti + 13] = vi + 3;
			tris[ti + 14] = vi + 7;
			tris[ti + 15] = vi + 2;
			tris[ti + 16] = vi + 7;
			tris[ti + 17] = vi + 6;

			tris[ti + 18] = vi + 4;
			tris[ti + 19] = vi + 5;
			tris[ti + 20] = vi + 9;
			tris[ti + 21] = vi + 4;
			tris[ti + 22] = vi + 9;
			tris[ti + 23] = vi + 8;

			tris[ti + 24] = vi + 6;
			tris[ti + 25] = vi + 7;
			tris[ti + 26] = vi + 11;
			tris[ti + 27] = vi + 6;
			tris[ti + 28] = vi + 11;
			tris[ti + 29] = vi + 10;

			tris[ti + 30] = vi + 8;
			tris[ti + 31] = vi + 9;
			tris[ti + 32] = vi + 13;
			tris[ti + 33] = vi + 8;
			tris[ti + 34] = vi + 13;
			tris[ti + 35] = vi + 12;

			tris[ti + 36] = vi + 9;
			tris[ti + 37] = vi + 10;
			tris[ti + 38] = vi + 14;
			tris[ti + 39] = vi + 9;
			tris[ti + 40] = vi + 14;
			tris[ti + 41] = vi + 13;

			tris[ti + 42] = vi + 10;
			tris[ti + 43] = vi + 11;
			tris[ti + 44] = vi + 15;
			tris[ti + 45] = vi + 10;
			tris[ti + 46] = vi + 15;
			tris[ti + 47] = vi + 14;

			if (fillInner) {
				tris[ti + 48] = vi + 5;
				tris[ti + 49] = vi + 6;
				tris[ti + 50] = vi + 10;
				tris[ti + 51] = vi + 5;
				tris[ti + 52] = vi + 10;
				tris[ti + 53] = vi + 9;
			}
		}

		static void WireGrid(int[] tris, int ti, int vi, int x, int y) {
			int x1 = x + 1;
			int x2 = x + 2;
			for (int yy = 0; yy < y; yy++) {
				for (int xx = 0; xx < x; xx++) {
					tris[ti++] = vi;
					tris[ti++] = vi + 1;
					tris[ti++] = vi + x2;
					tris[ti++] = vi;
					tris[ti++] = vi + x2;
					tris[ti++] = vi + x1;
					vi += 1;
				}
				vi += 1;
			}
		}
	}
}

