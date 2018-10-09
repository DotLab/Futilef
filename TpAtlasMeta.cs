namespace Futilef {
	public unsafe struct TpAtlasMeta {
		public int name;
		public fixed float size[2];

		public int spriteCount;
		public TpSpriteMeta *sprites;

		public static TpAtlasMeta *New(string str) {
			return Init((TpAtlasMeta *)Mem.Alloc(sizeof(TpAtlasMeta)), str);
		}

		public static TpAtlasMeta *Init(TpAtlasMeta *self, string str) {
			string[] segs = str.Split(',');
			int *nums = stackalloc int[segs.Length];
			for (int j = 0, end = segs.Length; j < end; j += 1) {
				nums[j] = int.Parse(segs[j]);
			}

			int i = 0;
			self->name = nums[i++];
			self->size[0] = nums[i++];
			self->size[1] = nums[i++];
			self->spriteCount = nums[i++];
			self->sprites = (TpSpriteMeta *)Mem.Alloc(self->spriteCount * sizeof(TpSpriteMeta));
			for (int j = 0, end = self->spriteCount; j < end; j += 1) {
				var sprite = self->sprites + j;
				sprite->atlas = self;
				sprite->name = nums[i++];
				sprite->rotated = nums[i++] != 0;
				sprite->size[0] = nums[i++];
				sprite->size[1] = nums[i++];
				sprite->pivot[0] = nums[i++];
				sprite->pivot[1] = nums[i++];
				sprite->quad[0] = nums[i++];
				sprite->quad[1] = nums[i++];
				sprite->quad[2] = nums[i++];
				sprite->quad[3] = nums[i++];
				sprite->uv[0] = nums[i++];
				sprite->uv[1] = nums[i++];
				sprite->uv[2] = nums[i++];
				sprite->uv[3] = nums[i++];
				sprite->border[0] = nums[i++];
				sprite->border[1] = nums[i++];
				sprite->border[2] = nums[i++];
				sprite->border[3] = nums[i++];
			}

			return self;
		}

		public static void Decon(TpAtlasMeta *self) {
			Mem.Free(self->sprites);
		}
	}

	public unsafe struct TpSpriteMeta {
		public TpAtlasMeta *atlas;

		public int name;
		public bool rotated;
		public fixed float size[2], pivot[2];
		public fixed float quad[4], uv[4], border[4];
	}
}