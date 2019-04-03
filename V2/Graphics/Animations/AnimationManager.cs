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
					sequence.finishTime = time + sequence.duration;
					sequence.Start();
					Console.Log("start sequence");
				}

				if (time >= sequence.finishTime) {  // started, finished
					Console.Log("finish sequence");
					sequence.Finish();
					var next = i.Next;
					sequenceList.Remove(i);
					i = next;
				} else {  // started, not finished
					sequence.Update(time - sequence.startTime);
					i = i.Next;
				}
			}
		}
	}
}