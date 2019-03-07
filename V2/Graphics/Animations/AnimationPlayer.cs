using System.Collections.Generic;

namespace Futilef.V2 {
	public sealed class AnimationPlayer {
		public readonly LinkedList<AnimationSequence> sequenceList = new LinkedList<AnimationSequence>();

		public AnimationSequence<T> Animate<T>(T target) where T : Drawable {
			var sequence = new AnimationSequence<T>(target);
			sequenceList.AddLast(sequence);
			return sequence;
		}

		public void Update(double time) {
			for (var i = sequenceList.First; i != null;) {
				var sequence = i.Value;

				if (!sequence.hasStarted) {  // not started
					sequence.hasStarted = true;
					sequence.Start(time);
					sequence.Update(time);
					i = i.Next;
				} else if (time > sequence.finishTime) {  // started, finished
					sequence.Finish();
					var next = i.Next;
					sequenceList.Remove(i);
					i = next;
					UnityEngine.Debug.Log("sequence finish");
				} else {  // started, not finished
					sequence.Update(time);
					i = i.Next;
				}
			}
		}
	}
}