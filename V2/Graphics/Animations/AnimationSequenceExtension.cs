namespace Futilef.V2 {
	public static class AnimationSequenceExtension {
		public static AnimationSequence<T> Delay<T>(this AnimationSequence<T> sequence, double time) where T : Drawable {
			sequence.nextTaskStartTime += time;
			return sequence;
		}
		public static AnimationSequence<T> Then<T>(this AnimationSequence<T> sequence) where T : Drawable {
			sequence.nextTaskStartTime = sequence.duration;
			return sequence;
		}
		public static AnimationSequence<T> Repeat<T>(this AnimationSequence<T> sequence, int count = -1) where T : Drawable {
			sequence.repeat = count;
			return sequence;
		}
		public static AnimationSequence<T> Insert<T>(this AnimationSequence<T> sequence, AnimationSequence s) where T : Drawable {
			sequence.Append(s);
			return sequence;
		}
		public static AnimationSequence<T> Insert<T>(this AnimationSequence<T> sequence, System.Action<AnimationSequence<T>> generator) where T : Drawable {
			var s = new AnimationSequence<T>(sequence.target); generator(s);
			sequence.Append(s);
			return sequence;
		}

		public static AnimationSequence<T> Scl<T>(this AnimationSequence<T> sequence, float x, float y, double duration = 0, int esType = 0) where T : Drawable {
			sequence.Append(new SclTask(sequence.target, duration, esType){end = new Vec2(x, y), setStartFromTarget = true}); return sequence;
		}
		public static AnimationSequence<T> Scl<T>(this AnimationSequence<T> sequence, float v, double duration = 0, int esType = 0) where T : Drawable {
			sequence.Append(new SclTask(sequence.target, duration, esType){end = new Vec2(v, v), setStartFromTarget = true}); return sequence;
		}

		public static AnimationSequence<T> Color<T>(this AnimationSequence<T> sequence, float v, double duration = 0, int esType = 0) where T : Drawable {
			sequence.Append(new ColorTask(sequence.target, duration, esType){end = new Vec4(v), setStartFromTarget = true}); 
			return sequence;
		}
		public static AnimationSequence<T> Color<T>(this AnimationSequence<T> sequence, float v, float a, double duration = 0, int esType = 0) where T : Drawable {
			sequence.Append(new ColorTask(sequence.target, duration, esType){end = new Vec4(v, a), setStartFromTarget = true}); 
			return sequence;
		}

		public static AnimationSequence<T> Alpha<T>(this AnimationSequence<T> sequence, float a, double duration = 0, int esType = 0) where T : Drawable {
			sequence.Append(new AlphaTask(sequence.target, duration, esType){end = a, setStartFromTarget = true}); return sequence;
		}
	}
}

