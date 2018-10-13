﻿namespace Futilef {
	public unsafe struct TpSpriteNode {
		public static int DepthCmp(void *a, void *b) {
			return ((TpSpriteNode *)a)->pos[2] - ((TpSpriteNode *)b)->pos[2] < 0 ? 1 : -1;
		}

		#if FDB
		public static readonly int Type = Fdb.NewType("TpSpriteNode");
		public int type;
		#endif

		public bool interactable;
		bool shouldRebuild;

		public fixed float pos[3];
		public fixed float scl[2];
		public float rot;

		public fixed float color[4];

		fixed float verts[4 * 2];
		fixed float uvs[4 * 2];

		TpSpriteMeta *spriteMeta;

		public static TpSpriteNode *New(TpSpriteMeta *spriteMeta) {
			#if FDB
			Should.NotNull("spriteMeta", spriteMeta);
			#endif
			return Init((TpSpriteNode *)Mem.Alloc(sizeof(TpSpriteNode)), spriteMeta);
		}

		public static TpSpriteNode *Init(TpSpriteNode *self, TpSpriteMeta *spriteMeta) {
			#if FDB
			Should.NotNull("self", self);
			Should.NotNull("spriteMeta", spriteMeta);
			self->type = Type;
			#endif
			self->interactable = false;
			self->shouldRebuild = true;

			Vec3.Zero(self->pos);
			Vec2.Set(self->scl, 1, 1);
			Vec4.Set(self->color, 1, 1, 1, 0);
			self->spriteMeta = spriteMeta;

			return self;
		}

		public static void Decon(TpSpriteNode *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			self->type = Fdb.NullType;
			#endif
		}

		public static void SetInteractable(TpSpriteNode *self, bool val) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			self->interactable = val;
		}

		public static void SetPosition(TpSpriteNode *self, float x, float y, float z) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			Vec3.Set(self->pos, x, y, z);
			self->shouldRebuild = true;
		}

		public static void SetRotation(TpSpriteNode *self, float rotation) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			self->rot = rotation;
			self->shouldRebuild = true;
		}

		public static void SetScale(TpSpriteNode *self, float x, float y) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			Vec2.Set(self->scl, x, y);
			self->shouldRebuild = true;
		}

		public static void SetAlpha(TpSpriteNode *self, float a) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			self->color[3] = a;
		}

		public static void SetTint(TpSpriteNode *self, float r, float g, float b) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
			Vec3.Set(self->color, r, g, b);
		}

		public static void Touch(TpSpriteNode *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			#endif
		}

		public static void Draw(TpSpriteNode *self) {
			#if FDB
			Should.NotNull("self", self);
			Should.TypeEqual("self", self->type, Type);
			Should.NotNull("self->spriteMeta", self->spriteMeta);
			#endif
			var bat = DrawCtx.GetBatch(self->spriteMeta->atlas->name);
			int vertIdx = bat.vertCount, triIdx = bat.triCount;
			bat.RequestQuota(4, 6);

			float *verts = self->verts;
			float *uvs = self->uvs;

			// v0 - v1
			// |  \ |
			// v3 - v2
			if (self->shouldRebuild) {
				float *mat = stackalloc float[6];
				Mat2D.FromScalingRotationTranslation(mat, self->pos, self->scl, self->rot);
				TpSpriteMeta.FillQuad(self->spriteMeta, self->verts, self->uvs, mat);
			}

			var bVerts = bat.verts; var bUvs = bat.uvs;
			float z = self->pos[2];
			bVerts[vertIdx    ].Set(verts[0], verts[1], z);
			bVerts[vertIdx + 1].Set(verts[2], verts[3], z);
			bVerts[vertIdx + 2].Set(verts[4], verts[5], z);
			bVerts[vertIdx + 3].Set(verts[6], verts[7], z);

			bUvs[vertIdx    ].Set(uvs[0], uvs[1]);
			bUvs[vertIdx + 1].Set(uvs[2], uvs[3]);
			bUvs[vertIdx + 2].Set(uvs[4], uvs[5]);
			bUvs[vertIdx + 3].Set(uvs[6], uvs[7]);

			float *color = self->color;
			var bColor = Vec4.Color(color, 0.5f);
			var bColors = bat.colors;
			bColors[vertIdx    ] = bColor;
			bColors[vertIdx + 1] = bColor;
			bColors[vertIdx + 2] = bColor;
			bColors[vertIdx + 3] = bColor;

			var bTris = bat.tris;
			bTris[triIdx]     = vertIdx;
			bTris[triIdx + 1] = vertIdx + 1;
			bTris[triIdx + 2] = vertIdx + 2;
			bTris[triIdx + 3] = vertIdx;
			bTris[triIdx + 4] = vertIdx + 2;
			bTris[triIdx + 5] = vertIdx + 3;
		}
	}
}
