namespace Futilef.V2 {
	public abstract class AnimationTask {
		public bool hasStarted;

		public double duration;
		public double startTime;
		public double finishTime;
		public double durationRecip;

		public int esType;

		protected AnimationTask(double duration, int esType) {
			this.duration = duration;
			durationRecip = 1.0 / duration;
			this.esType = esType;
		}

		public void Update(double time) {
			Apply(Es.Calc(esType, (time - startTime) * durationRecip));
		}

		public abstract void Start();
		public abstract void Apply(double t);
		public abstract void Finish();
	}

	public abstract class AnimationTask <T> : AnimationTask where T : Drawable {
		public readonly T target;

		protected AnimationTask(T target, double duration, int esType) : base(duration, esType) {
			this.target = target;
		}
	}

	public sealed class AlphaTask : AnimationTask<Drawable> {
		public float start, end, delta;
		public bool setStartFromTarget;
		public AlphaTask(Drawable target, double duration, int esType) : base(target, duration, esType) {}
		public override void Start() { 
			if (setStartFromTarget) start = target.alpha; 
			delta = end - start;
		}
		public override void Apply(double t) { target.alpha = start + delta * (float)t; target.hasColorChanged = true; }
		public override void Finish() { target.alpha = end; target.hasColorChanged = true; }
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
		public override void Apply(double t) { target.color = start + delta * (float)t; target.hasColorChanged = true; }
		public override void Finish() { target.color = end; target.hasColorChanged = true; }
	}

	public sealed class RotTask : AnimationTask<Drawable> {
		public float start, end, delta;
		public bool setStartFromTarget, setEndFromTarget, isRelative;
		public RotTask(Drawable target, double duration, int esType) : base(target, duration, esType) {}
		public override void Start() { 
			if (setStartFromTarget) start = target.rot; 
			if (setEndFromTarget) end = target.rot; 
			if (isRelative) { start = target.rot; end = target.rot + end; }
			delta = end - start; 
		}
		public override void Apply(double t) { target.rot = start + delta * (float)t; target.hasTransformChanged = true; }
		public override void Finish() { target.rot = end; target.hasTransformChanged = true; }
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
		public override void Apply(double t) { target.scl = start + delta * (float)t; target.hasTransformChanged = true; }
		public override void Finish() { target.scl = end; target.hasTransformChanged = true; }
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
			     if (applyXOnly) target.size.x = value.x;
			else if (applyYOnly) target.size.y = value.y;
			else                 target.size = value;
			target.hasTransformChanged = true; 
		}
		public override void Finish() { 
			     if (applyXOnly) target.size.x = end.x;
			else if (applyYOnly) target.size.y = end.y;
			else                 target.size = end;
			target.hasTransformChanged = true; 
		}
	}
}

