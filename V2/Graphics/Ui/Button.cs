namespace Futilef.V2 {
	public class Button : CompositeDrawable {
		public readonly BmLabel text;
		public readonly TpSprite background;
		public readonly AnimationManager animationManager;

		public Button(AnimationManager animationManager, TpSprite background, BmLabel text) {
			this.animationManager = animationManager;
			this.background = background;
			this.text = text;

			handleInput = true;
			useLayout = true;
			size.One();

			background.useLayout = true;
			background.size.One();
			background.relativeSizeAxes = Axes.Both;
			background.anchorAlign = Align.Center;
			background.pivotAlign = Align.Center;
			background.transformDirty = true;

			text.useLayout = true;
			text.size.One();
			text.relativeSizeAxes = Axes.Both;
			text.verticalAlign = BmLabel.VerticalAlign.Mesh;
			text.textAlign = Align.Center;
			text.anchorAlign = Align.Center;
			text.pivotAlign = Align.Center;
			text.transformDirty = true;

			children.Add(background);
			children.Add(text);
		}

		public override bool OnTouchDown(TouchDownEvent e) {
//			var matInverse = new Mat2D();
//			matInverse.FromInverse(cachedMatConcat);
			var screenQuad = cachedMatConcat * new Quad(cachedPos.x, cachedPos.y, cachedSize.x, cachedSize.y);
			UnityEngine.Debug.LogFormat("{0} {1} {2} {3} {4} {5}", screenQuad.bl, screenQuad.br, screenQuad.tl, screenQuad.tr, e.pos, screenQuad.GetAabb());
			if (screenQuad.GetAabb().Contains(e.pos)) {
				animationManager.Animate(this)
					.FadeOut(.2, EsType.CubicOut)
					.MoveYTo(1, 0.2, EsType.CubicOut)
					.RotTo(.5f, 0.2, EsType.CubicOut).Then()
					.MoveYTo(0, 0.2, EsType.CubicIn)
					.RotTo(0, 0.2, EsType.CubicIn).Then()
					.FadeIn(1, EsType.CubicOut)
					.Spin(3.125f, 1, EsType.CubicOut);
				return true;
			}
			return false;
		}
	}
}

