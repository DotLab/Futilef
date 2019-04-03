﻿using System.Collections.Generic;

namespace Futilef.V2 {
	public abstract class AnimationSequence : AnimationTask {
		public readonly List<AnimationTask> taskList = new List<AnimationTask>();
		public int repeat;

		public double nextTaskStartTime;

		int index;
		int repeatIndex;

		public void Append(AnimationTask task) {
			task.startTime = nextTaskStartTime;
			task.finishTime = nextTaskStartTime + task.duration;
			taskList.Add(task);
			if (duration < task.finishTime) duration = task.finishTime;
		}

		public override void Start() {
			index = 0;
			repeatIndex = 0;
		}

		public override void Update(double time) {
			for (int i = index, end = taskList.Count; i < end; i += 1) {
				var task = taskList[i];
				if (task.startTime > time) break;  // should not start

				if (!task.hasStarted) {  // not started
					task.hasStarted = true;
					task.Start();
				} 

				if (time >= task.finishTime) {  // started, finished
					task.Finish();
					index += 1;
				} else {  // started, not finished
					task.Update(time - task.startTime);
				}
			}
		}

		public override void Finish() {
			for (int i = index, end = taskList.Count; i < end; i += 1) {
				taskList[i].Finish();
				index += 1;
			}
		}
	}

	public class AnimationSequence <T> : AnimationSequence where T : Drawable {
		public readonly T target;

		public AnimationSequence(T target) {
			this.target = target;
		}
	}

	public static class AnimationSequenceExtension1 {
		public static AnimationSequence<T> FadeIn<T>(this AnimationSequence<T> sequence, double duration, int esType) where T : Drawable {
			sequence.Append(new AlphaTask(sequence.target, duration, esType){end = 1, setStartFromTarget = true}); 
			return sequence;
		}
		public static AnimationSequence<T> FadeInFromZero<T>(this AnimationSequence<T> sequence, double duration, int esType) where T : Drawable {
			sequence.Append(new AlphaTask(sequence.target, duration, esType){start = 0, end = 1}); 
			return sequence;
		}
		public static AnimationSequence<T> FadeOut<T>(this AnimationSequence<T> sequence, double duration, int esType) where T : Drawable {
			sequence.Append(new AlphaTask(sequence.target, duration, esType){end = 0, setStartFromTarget = true}); 
			return sequence;
		}
		public static AnimationSequence<T> FadeOutFromOne<T>(this AnimationSequence<T> sequence, double duration, int esType) where T : Drawable {
			sequence.Append(new AlphaTask(sequence.target, duration, esType){start = 1, end = 0}); 
			return sequence;
		}
		public static AnimationSequence<T> FadeColorTo<T>(this AnimationSequence<T> sequence, Vec4 color, double duration, int esType) where T : Drawable {
			sequence.Append(new ColorTask(sequence.target, duration, esType){end = color, setStartFromTarget = true}); 
			return sequence;
		}
		public static AnimationSequence<T> FlashColor<T>(this AnimationSequence<T> sequence, Vec4 color, double duration, int esType) where T : Drawable {
			sequence.Append(new ColorTask(sequence.target, duration, esType){start = color, setEndFromTarget = true}); 
			return sequence;
		}

		public static AnimationSequence<T> Spin<T>(this AnimationSequence<T> sequence, float count, double duration, int esType) where T : Drawable {
			sequence.Append(new RotTask(sequence.target, duration, esType){end = (float)Es.TwoPi * count, isRelative = true}); 
			return sequence;
		}

		public static AnimationSequence<T> RotateTo<T>(this AnimationSequence<T> sequence, float rot, double duration, int esType) where T : Drawable {
			sequence.Append(new RotTask(sequence.target, duration, esType){end = rot, setStartFromTarget = true}); 
			return sequence;
		}

		public static AnimationSequence<T> ScaleTo<T>(this AnimationSequence<T> sequence, Vec2 scl, double duration, int esType) where T : Drawable {
			sequence.Append(new SclTask(sequence.target, duration, esType){end = scl, setStartFromTarget = true}); 
			return sequence;
		}

		public static AnimationSequence<T> ResizeTo<T>(this AnimationSequence<T> sequence, Vec2 size, double duration, int esType) where T : Drawable {
			sequence.Append(new SizeTask(sequence.target, duration, esType){end = size, setStartFromTarget = true}); 
			return sequence;
		}
		public static AnimationSequence<T> ResizeWidthTo<T>(this AnimationSequence<T> sequence, float width, double duration, int esType) where T : Drawable {
			sequence.Append(new SizeTask(sequence.target, duration, esType){end = new Vec2(width, 0), setStartFromTarget = true, applyXOnly = true}); 
			return sequence;
		}
		public static AnimationSequence<T> ResizeHeightTo<T>(this AnimationSequence<T> sequence, float height, double duration, int esType) where T : Drawable {
			sequence.Append(new SizeTask(sequence.target, duration, esType){end = new Vec2(0, height), setStartFromTarget = true, applyYOnly = true}); 
			return sequence;
		}
			
		public static AnimationSequence<T> MoveTo<T>(this AnimationSequence<T> sequence, Vec2 pos, double duration, int esType) where T : Drawable {
			sequence.Append(new PosTask(sequence.target, duration, esType){end = pos, setStartFromTarget = true}); 
			return sequence;
		}
		public static AnimationSequence<T> MoveXTo<T>(this AnimationSequence<T> sequence, float x, double duration, int esType) where T : Drawable {
			sequence.Append(new PosTask(sequence.target, duration, esType){end = new Vec2(x, 0), setStartFromTarget = true, applyXOnly = true}); 
			return sequence;
		}
		public static AnimationSequence<T> MoveYTo<T>(this AnimationSequence<T> sequence, float y, double duration, int esType) where T : Drawable {
			sequence.Append(new PosTask(sequence.target, duration, esType){end = new Vec2(0, y), setStartFromTarget = true, applyYOnly = true}); 
			return sequence;
		}
	}
}

