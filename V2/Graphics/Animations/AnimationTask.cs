namespace Futilef.V2 {
	public abstract class AnimationTask {
		public double duration;
		public bool durationInfinite;

		public bool hasStarted;
		public bool hasFinished;
		public double startTime;  // parent time

		public abstract void Start();
		public abstract void Update(double time);  // local time
		public abstract void Finish();
	}

	public abstract class AnimationTask <T> : AnimationTask where T : Drawable {
		public readonly T target;
		public double durationRecip;
		public int esType;

		protected AnimationTask(T target, double duration, int esType) {
			this.target = target;
			this.duration = duration;
			durationRecip = 1.0 / duration;
			this.esType = esType;
		}

		public override void Update(double time) {
			Apply(Es.Calc(esType, time * durationRecip));

			if (time >= duration) {
//				Console.Log("task finish");
				Finish();
				hasFinished = true;
			}
		}

		public abstract void Apply(double t);
	}

	public sealed class SclTask : AnimationTask<Drawable> {
		public Vec2 start, end, delta;
		public bool setStartFromTarget, setEndFromTarget;
		public SclTask(Drawable target, double duration, int esType) : base(target, duration, esType) {}
		public override void Start() {
			if (setStartFromTarget) start = target.scl; 
			if (setEndFromTarget) end = target.scl; 
			delta = end - start; 
		}
		public override void Apply(double t) { target.Scl(start + delta * (float)t); }
		public override void Finish() { target.Scl(end); }
	}

	public sealed class AlphaTask : AnimationTask<Drawable> {
		public float start, end, delta;
		public bool setStartFromTarget;
		public AlphaTask(Drawable target, double duration, int esType) : base(target, duration, esType) {}
		public override void Start() {
			if (setStartFromTarget) start = target.alpha; 
			delta = end - start;
		}
		public override void Apply(double t) { target.Alpha(start + delta * (float)t); }
		public override void Finish() { target.Alpha(end); }
	}

	public sealed class ColorTask : AnimationTask<Drawable> {
		public Vec4 start, end, delta;
		public bool setStartFromTarget, setEndFromTarget;
		public ColorTask(Drawable target, double duration, int esType) : base(target, duration, esType) {}
		public override void Start() { 
			if (setStartFromTarget) start = target.color; 
			if (setEndFromTarget) end = target.color; 
			delta = end - start; 
		}
		public override void Apply(double t) { target.color = start + delta * (float)t; target.colorDirty = true; target.age += 1; }
		public override void Finish() { target.color = end; target.colorDirty = true; target.age += 1; }
	}

	public sealed class RotTask : AnimationTask<Drawable> {
		public float start, end, delta;
		public bool setStartFromTarget, setEndFromTarget, isRelative;
		public RotTask(Drawable target, double duration, int esType) : base(target, duration, esType) {}
		public override void Start() { 
			if (setStartFromTarget) start = target.rot; 
			if (setEndFromTarget) end = target.rot;
			if (isRelative) { start = target.rot; end = target.rot + delta; }
			else delta = end - start; 
		}
		public override void Apply(double t) { target.rot = start + delta * (float)t; target.transformDirty = true; target.age += 1; }
		public override void Finish() { target.rot = end; target.transformDirty = true; target.age += 1; }
	}

	public sealed class SizeTask : AnimationTask<Drawable> {
		public Vec2 start, end, delta;
		public bool setStartFromTarget, setEndFromTarget, applyXOnly, applyYOnly;
		public SizeTask(Drawable target, double duration, int esType) : base(target, duration, esType) {}
		public override void Start() { 
			if (setStartFromTarget) start = target.size; 
			if (setEndFromTarget) end = target.size; 
			delta = end - start; 
		}
		public override void Apply(double t) {
			var value = start + delta * (float)t; 
			if (applyXOnly)      target.size.x = value.x;
			else if (applyYOnly) target.size.y = value.y;
			else                 target.size = value;
			target.transformDirty = true; target.age += 1; 
		}
		public override void Finish() { 
			if (applyXOnly)      target.size.x = end.x;
			else if (applyYOnly) target.size.y = end.y;
			else                 target.size = end;
			target.transformDirty = true; target.age += 1; 
		}
	}

	public sealed class PosTask : AnimationTask<Drawable> {
		public Vec2 start, end, delta;
		public bool setStartFromTarget, setEndFromTarget, applyXOnly, applyYOnly;
		public PosTask(Drawable target, double duration, int esType) : base(target, duration, esType) {}
		public override void Start() { 
			if (setStartFromTarget) start = target.pos; 
			if (setEndFromTarget) end = target.pos; 
			delta = end - start; 
		}
		public override void Apply(double t) {
			var value = start + delta * (float)t; 
			if (applyXOnly)      target.pos.x = value.x;
			else if (applyYOnly) target.pos.y = value.y;
			else                 target.pos = value;
			target.transformDirty = true; target.age += 1; 
		}
		public override void Finish() { 
			if (applyXOnly)      target.pos.x = end.x;
			else if (applyYOnly) target.pos.y = end.y;
			else                 target.pos = end;
			target.transformDirty = true; target.age += 1; 
		}
	}
}

