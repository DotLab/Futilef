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
			gameObject.SetActive(true);
		}

		public void Deactivate() {
			gameObject.SetActive(false);
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
		public static void DrawQuad(this DrawBat bat, Quad q, Quad uvQ, Vec4 c) {
			int vi = bat.vertCount;
			int ti = bat.triCount;
			bat.RequestQuota(4, 2);

			var verts = bat.verts;
			verts[vi + 0].Set(q.tl.x, q.tl.y, 0);
			verts[vi + 1].Set(q.tr.x, q.tr.y, 0);
			verts[vi + 2].Set(q.br.x, q.br.y, 0);
			verts[vi + 3].Set(q.bl.x, q.bl.y, 0);

			var uvs = bat.uvs;
			uvs[vi + 0].Set(uvQ.tl.x, uvQ.tl.y);
			uvs[vi + 1].Set(uvQ.tr.x, uvQ.tr.y);
			uvs[vi + 2].Set(uvQ.br.x, uvQ.br.y);
			uvs[vi + 3].Set(uvQ.bl.x, uvQ.bl.y);

			var color = new Color(c.x, c.y, c.z, c.w);
			var colors = bat.colors;
			colors[vi + 0] = color;
			colors[vi + 1] = color;
			colors[vi + 2] = color;
			colors[vi + 3] = color;

			var tris = bat.tris;
			tris[ti + 0] = vi;
			tris[ti + 1] = vi + 1;
			tris[ti + 2] = vi + 2;
			tris[ti + 3] = vi;
			tris[ti + 4] = vi + 2;
			tris[ti + 5] = vi + 3;
		}
	}
}

