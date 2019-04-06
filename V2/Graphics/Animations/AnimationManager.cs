using System.Collections.Generic;

namespace Futilef.V2 {
	public sealed class AnimationManager {
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
					sequence.startTime = time;
					sequence.Start();
				}

				sequence.Update(time - sequence.startTime);

				if (sequence.hasFinished) {  // started, finished
//					Console.Log("sequence finish");
					var next = i.Next;
					sequenceList.Remove(i);
					i = next;
				} else {  // started, not finished
					i = i.Next;
				}
			}
		}
	}
}