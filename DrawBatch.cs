using Array = System.Array;
using UnityEngine;

namespace Futilef {
	public class DrawBatch {
		const int VertExpandAmount = 60, TriExpandAmount = 30;
		const int VertUnusedLimit = 600, TriUnusedLimit = 300;

		public Shader shader;
		public Texture2D texture;

		public GameObject gameObject;

		public MeshFilter meshFilter;
		public Mesh mesh;

		public MeshRenderer meshRenderer;
		public Material material;

		public int vertLen = VertExpandAmount, vertCount;
		public Vector3[] verts = new Vector3[VertExpandAmount];
		public Color[] colors = new Color[VertExpandAmount];
		public Vector2[] uvs = new Vector2[VertExpandAmount];

		public int triLen = TriExpandAmount, triCount;
		public int[] tris = new int[TriExpandAmount];

		public DrawBatch(Shader shader, Texture2D texture) {
			this.shader = shader; this.texture = texture;

			gameObject = new GameObject(string.Format("Batch[{0}|{1}x{2}]", shader.name, texture.width, texture.height));

			meshFilter = gameObject.AddComponent<MeshFilter>();
			mesh = meshFilter.mesh;
			mesh.MarkDynamic();

			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			material = new Material(shader) { mainTexture = texture };
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

			if ((triCount += t) >= triLen) {
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

			mesh.RecalculateBounds();
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
}
