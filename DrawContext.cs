using System.Collections.Generic;
using UnityEngine;

namespace Futilef {
	public class DrawContext {
		const int BaseQueue = 3001;
		static readonly Shader DefaultShader = Shader.Find("Futilef/Basic");

		public LinkedList<DrawBatch> prevBatches = new LinkedList<DrawBatch>();
		public LinkedList<DrawBatch> activeBatches = new LinkedList<DrawBatch>();
		public readonly LinkedList<DrawBatch> inactiveBatches = new LinkedList<DrawBatch>();

		int curQueue;
		DrawBatch curBatch;

		public void Start() {
			curQueue = BaseQueue;
			curBatch = null;
		}

		public void Finish() {
			for (var node = prevBatches.First; prevBatches.Count > 0; node = prevBatches.First) {
				node.Value.Deactivate();
				inactiveBatches.AddLast(node.Value);
				prevBatches.RemoveFirst();
			}

			// Same as prevBatches.AddRange(activeBatches); activeBatches.Clear();
			var swap = prevBatches;
			prevBatches = activeBatches;
			activeBatches = swap;

			if (curBatch != null) {
				curBatch.Close();
				curBatch = null;
			}
		}

		public DrawBatch GetBatch(int textureId) {
			return GetBatch(DefaultShader, Res.GetTexture(textureId));
		}

		public DrawBatch GetBatch(Shader shader, Texture2D texture) {
			if (curBatch != null) {
				if (curBatch.shader == shader && curBatch.texture == texture) return curBatch;
				curBatch.Close();
			}

			curQueue += 1;

			// if there is a prevBatch that matches
			for (var node = prevBatches.First; node != null; node = node.Next) {
				curBatch = node.Value;
				if (curBatch.shader == shader && curBatch.texture == texture) {
					prevBatches.Remove(node);
					curBatch.Open(curQueue);
					activeBatches.AddLast(curBatch);
					return curBatch;
				}
			}

			// if there is an inactiveBatch that matches
			for (var node = inactiveBatches.First; node != null; node = node.Next) {
				curBatch = node.Value;
				if (curBatch.shader == shader && curBatch.texture == texture) {
					inactiveBatches.Remove(node);
					curBatch.Activate();
					curBatch.Open(curQueue);
					activeBatches.AddLast(curBatch);
					return curBatch;
				}
			}

			// create a new batch
			curBatch = new DrawBatch(shader, texture);
			curBatch.Open(curQueue);
			activeBatches.AddLast(curBatch);
			return curBatch;
		}

		public void Dispose() {
			foreach (var batch in activeBatches) batch.Dispose();
			foreach (var batch in prevBatches) batch.Dispose();
			foreach (var batch in inactiveBatches) batch.Dispose();
		}
	}
}
