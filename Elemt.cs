namespace Futilef {
	public static unsafe class Elemt {
		public static bool IsDepthDirty(void *elemt) {
			switch (*(int *)elemt) {
			case Tag.TpSprite: return ((TpSprite *)elemt)->isDepthDirty;
			case Tag.Group: return ((Group *)elemt)->isDepthDirty;
			}
			return false;
		}

		public static float *GetPosition(void *elemt) {
			switch (*(int *)elemt) {
			case Tag.TpSprite: return ((TpSprite *)elemt)->pos;
			case Tag.Group: return ((Group *)elemt)->pos;
			}
			return null;
		}

		public static void Draw(void *elemt, float *parentMat, bool isParentTransformDirty) {
			switch (*(int *)elemt) {
			case Tag.TpSprite: TpSprite.Draw((TpSprite *)elemt, parentMat, isParentTransformDirty); return;
			case Tag.Group: Group.Draw((Group *)elemt, parentMat, isParentTransformDirty); return;
			}
		}

		public static int DepthCmp(void *elemt1, void *elemt2) {
			float depth1 = GetPosition(elemt1)[2], depth2 = GetPosition(elemt2)[2];
			return depth1 - depth2 < 0 ? 1 : -1;
		}
	}
}

