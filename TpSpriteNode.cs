namespace Futilef {
	public unsafe struct TpSpriteNode {
		public static int DepthCmp(void *a, void *b) {
			return ((TpSpriteNode *)a)->pos[2] - ((TpSpriteNode *)b)->pos[2] < 0 ? -1 : 1;
		}

		public int type;

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
			return Init((TpSpriteNode *)Mem.Alloc(sizeof(TpSpriteNode)), spriteMeta);
		}

		public static TpSpriteNode *Init(TpSpriteNode *self, TpSpriteMeta *spriteMeta) {
			self->type = 1;
			self->interactable = false;
			self->shouldRebuild = true;

			Vec3.Zero(self->pos);
			Vec2.Set(self->scl, 1, 1);
			Vec4.Set(self->color, 1, 1, 1, 0);
			self->spriteMeta = spriteMeta;

			return self;
		}

		public static void SetInteractable(TpSpriteNode *self, bool val) {
			self->interactable = val;
		}

		public static void SetPosition(TpSpriteNode *self, float x, float y, float z) {
			Vec3.Set(self->pos, x, y, z);
			self->shouldRebuild = true;
		}

		public static void SetRotation(TpSpriteNode *self, float rotation) {
			self->rot = rotation;
			self->shouldRebuild = true;
		}

		public static void SetScale(TpSpriteNode *self, float x, float y) {
			Vec2.Set(self->scl, x, y);
			self->shouldRebuild = true;
		}

		public static void SetAlpha(TpSpriteNode *self, float a) {
			self->color[3] = a;
		}

		public static void SetTint(TpSpriteNode *self, float r, float g, float b) {
			Vec3.Set(self->color, r, g, b);
		}

		public static void Touch(TpSpriteNode *self) {
			
		}

		public static void Draw(TpSpriteNode *self) {
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

				float *pivot = self->spriteMeta->pivot;
				float pivotX = pivot[0], pivotY = pivot[1];
				float *quad = self->spriteMeta->quad;
				float quadX = quad[0], quadY = quad[1], quadW = quad[2], quadH = quad[3];
				Vec2.TransformMat2D(verts,     mat, -pivotX + quadX,         pivotY - quadY);
				Vec2.TransformMat2D(verts + 2, mat, -pivotX + quadX + quadW, pivotY - quadY);
				Vec2.TransformMat2D(verts + 4, mat, -pivotX + quadX + quadW, pivotY - quadY - quadH);
				Vec2.TransformMat2D(verts + 6, mat, -pivotX + quadX,         pivotY - quadY - quadH);

				float *size = self->spriteMeta->atlas->size;
				float invSizeX = 1 / size[0], invSizeY = 1 / size[1];
				float *uv = self->spriteMeta->uv;
				float uvX = uv[0], uvY = uv[1], uvW = uv[2], uvH = uv[3];
				if (self->spriteMeta->rotated) {
					Vec2.Set(uvs    , (uvX + uvW) * invSizeX,  -uvY        * invSizeY);
					Vec2.Set(uvs + 2, (uvX + uvW) * invSizeX, (-uvY - uvH) * invSizeY);
					Vec2.Set(uvs + 4,  uvX        * invSizeX, (-uvY - uvH) * invSizeY);
					Vec2.Set(uvs + 6,  uvX        * invSizeX,  -uvY        * invSizeY);
				} else {
					Vec2.Set(uvs    ,  uvX        * invSizeX,  -uvY        * invSizeY);
					Vec2.Set(uvs + 2, (uvX + uvW) * invSizeX,  -uvY        * invSizeY);
					Vec2.Set(uvs + 4, (uvX + uvW) * invSizeX, (-uvY - uvH) * invSizeY);
					Vec2.Set(uvs + 6,  uvX        * invSizeX, (-uvY - uvH) * invSizeY);
				}
			}

			var bVerts = bat.verts; var bUvs = bat.uvs; 
			bVerts[vertIdx    ].Set(verts[0], verts[1], 0);
			bVerts[vertIdx + 1].Set(verts[2], verts[3], 0);
			bVerts[vertIdx + 2].Set(verts[4], verts[5], 0);
			bVerts[vertIdx + 3].Set(verts[6], verts[7], 0);

			bUvs[vertIdx    ].Set(uvs[0], uvs[1]);
			bUvs[vertIdx + 1].Set(uvs[2], uvs[3]);
			bUvs[vertIdx + 2].Set(uvs[4], uvs[5]);
			bUvs[vertIdx + 3].Set(uvs[6], uvs[7]);

			float *color = self->color;
			var bColor = Vec4.Color(color);
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
