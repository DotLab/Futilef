using System.Collections.Generic;

namespace Futilef.V2 {
	public sealed class DrawCtx : System.IDisposable {
		struct Group {
			public int queue;
			public LinkedList<DrawBat> batches;

			public Group(int queue) {
				this.queue = queue;
				batches = new LinkedList<DrawBat>();
			}
		}

		public const int BackgroundQueue = 1000, GeometryQueue = 2000, TransparentQueue = 3000, OverlayQueue = 4000;

		LinkedList<DrawBat> prevBatches = new LinkedList<DrawBat>();
		LinkedList<DrawBat> activeBatches = new LinkedList<DrawBat>();
		readonly LinkedList<DrawBat> inactiveBatches = new LinkedList<DrawBat>();

		readonly List<Group> groups = new List<Group>();
		Group curGroup;

		int curQueue;
		DrawBat curBatch;

		public readonly Shader debugShader = new Shader("Debug", UnityEngine.Shader.Find("Sprites/Default"));
		public readonly Texture debugTexture = new Texture("Debug", new UnityEngine.Texture2D(2, 2));

		public void Start() {
			curQueue = TransparentQueue + 1;
			curBatch = null;
		}

		public void Finish() {
			for (var node = activeBatches.First; node != null; node = node.Next) {
				node.Value.Close();
			}

			for (var node = prevBatches.First; prevBatches.Count > 0; node = prevBatches.First) {
				node.Value.Deactivate();
				inactiveBatches.AddLast(node.Value);
				prevBatches.RemoveFirst();
			}

			// Same as prevBatches.AddRange(activeBatches); activeBatches.Clear();
			var swap = prevBatches;
			prevBatches = activeBatches;
			activeBatches = swap;

			curBatch = null;
		}

		public int NewGroup() {
			int res = groups.Count;
			groups.Add(new Group(curQueue += 1));
			return res;
		}

		public int NewGroup(int queue) {
			int res = groups.Count;
			groups.Add(new Group(queue));
			return res;
		}

		public DrawBat GetBatch(Shader shader, Texture texture, int g) {
			DrawBat batch;

			var batches = groups[g].batches;
			for (var node = batches.First; node != null; node = node.Next) {
				batch = node.Value;
				if (batch.shader == shader && batch.texture == texture) {
					return batch;
				}
			}

			batch = FindOrCreateBatch(shader, texture, groups[g].queue);
			groups[g].batches.AddFirst(batch);
			return batch;
		}

		public DrawBat GetBatch(Shader shader, Texture texture) {
			if (curBatch != null && curBatch.shader == shader && curBatch.texture == texture) return curBatch;
			return curBatch = FindOrCreateBatch(shader, texture, curQueue += 1);
		}

		DrawBat FindOrCreateBatch(Shader shader, Texture texture, int queue) {
			DrawBat batch;

			// if there is a prevBatch that matches
			for (var node = prevBatches.First; node != null; node = node.Next) {
				batch = node.Value;
				if (batch.shader == shader && batch.texture == texture) {
					prevBatches.Remove(node);
					batch.Open(queue);
					activeBatches.AddLast(batch);
					return batch;
				}
			}

			// if there is an inactiveBatch that matches
			for (var node = inactiveBatches.First; node != null; node = node.Next) {
				batch = node.Value;
				if (batch.shader == shader && batch.texture == texture) {
					inactiveBatches.Remove(node);
					batch.Activate();
					batch.Open(queue);
					activeBatches.AddLast(batch);
					return batch;
				}
			}

			// create a new batch
			batch = new DrawBat(shader, texture);
			batch.Open(queue);
			activeBatches.AddLast(batch);
			return batch;
		}

		public void Dispose() {
			foreach (var batch in activeBatches) batch.Dispose(); activeBatches.Clear();
			foreach (var batch in prevBatches) batch.Dispose(); prevBatches.Clear();
			foreach (var batch in inactiveBatches) batch.Dispose(); inactiveBatches.Clear();
		}
	}
}
