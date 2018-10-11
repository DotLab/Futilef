using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace Futilef {
	public unsafe class GpController {
		class Cmd { }

		class WaitCmd : Cmd { public float time; }

		class ImgCmd : Cmd { public int id; }
		class AddImgCmd : ImgCmd { public int imgId; }
		class RmImgCmd : ImgCmd { }
		class SetImgAttrCmd : ImgCmd { public int imgAttrId; public object[] args; }
		class SetImgAttrEasedCmd : SetImgAttrCmd { public float duration; public int esType; }

		class SetCamAttrCmd : Cmd { public int camAttrId; public object[] args; }
		class SetCamAttrEasedCmd : SetCamAttrCmd { public float duration; public int esType; }
		
		public static class ImgAttr { public const int Interactable = 0, Position = 1, Rotation = 2, Scale = 3, Alpha = 4, Tint = 5, ImgId = 6; }

		class EsJob { public float time, duration; public int esType; public TpSpriteNode *node; public virtual void Apply(float step) {} public virtual void Finish() {} }
		class EsSetPositionJob : EsJob { public float x, y, z, dx, dy, dz; public override void Apply(float step) { TpSpriteNode.SetPosition(node, x + dx * step, y + dy * step, z + dz * step); } public override void Finish() { TpSpriteNode.SetPosition(node, x + dx, y + dy, z + dz); } }
		class EsSetRotationJob : EsJob { public float r, dr;               public override void Apply(float step) { TpSpriteNode.SetRotation(node, r + dr * step); }                               public override void Finish() { TpSpriteNode.SetRotation(node, r + dr); } }
		class EsSetScaleJob    : EsJob { public float x, dx, y, dy;        public override void Apply(float step) { TpSpriteNode.SetScale(node, x + dx * step, y + dy * step); }                   public override void Finish() { TpSpriteNode.SetScale(node, x + dx, y + dy); } }
		class EsSetAlphaJob    : EsJob { public float a, da;               public override void Apply(float step) { TpSpriteNode.SetAlpha(node, a + da * step); }                                  public override void Finish() { TpSpriteNode.SetAlpha(node, a + da); } }
		class EsSetTintJob     : EsJob { public float r, g, b, dr, dg, db; public override void Apply(float step) { TpSpriteNode.SetTint(node, r + dr * step, g + dg * step, b + db * step); }     public override void Finish() { TpSpriteNode.SetTint(node, r + dr, g + dg, b + db); } }

		readonly Queue<Cmd> cmdQueue = new Queue<Cmd>();
		readonly LinkedList<EsJob> esJobList = new LinkedList<EsJob>();
		readonly Dictionary<int, int> nodeIdxDict = new Dictionary<int, int>();

		float time;
		float waitEndTime = -1, lastEsEndTime;

		Lst *spriteNodeLst = Lst.New(sizeof(TpSpriteNode));
		PtrLst *spriteNodePtrLst = PtrLst.New();

		bool needDepthSort;

		public void Init() {
			Debug.Log("Init GPC");
		}

		public void Dispose() {
			Lst.Decon(spriteNodeLst); Mem.Free(spriteNodeLst);
			PtrLst.Decon(spriteNodePtrLst); Mem.Free(spriteNodePtrLst);
			DrawCtx.Dispose();

			Debug.Log("Clean up GPC");
		}

		public void Update(float deltaTime) {
			time += deltaTime;

			// execute commands
			if (time > waitEndTime) {
				while (time > waitEndTime && cmdQueue.Count > 0) {
					var cmd = cmdQueue.Dequeue();
					
					switch (cmd.GetType().Name) {
						case "WaitCmd":            Wait(cmd as WaitCmd); break;
						case "AddImgCmd":          AddImg(cmd as AddImgCmd); break;
						case "RmImgCmd":           RmImg(cmd as RmImgCmd); break;
						case "SetImgAttrCmd":      SetImgAttr(cmd as SetImgAttrCmd); break;
						case "SetImgAttrEasedCmd": SetImgAttrEased(cmd as SetImgAttrEasedCmd); break;
						case "SetCamAttrCmd":      SetCamAttr(cmd as SetCamAttrCmd); break;
						case "SetCamAttrEasedCmd": SetCamAttrEased(cmd as SetCamAttrEasedCmd); break;
					}
				}	
			}

			// execute easing jobs
			for (var node = esJobList.First; node != null;) {
				var next = node.Next;
				var job = node.Value;
				if ((job.time += deltaTime) > job.duration) {
					job.Finish();
					esJobList.Remove(node);
				} else {
					job.Apply(Es.Ease(job.esType, job.time / job.duration));
				}
				node = next;
			}

			// draw
			if (needDepthSort) { needDepthSort = false; PtrLst.Qsort(spriteNodePtrLst, TpSpriteNode.DepthCmp); }
			DrawCtx.Start();
			var arr = (TpSpriteNode **)spriteNodePtrLst->arr;
			for (int i = 0, end = spriteNodePtrLst->count; i < end; i += 1) TpSpriteNode.Draw(arr[i]);
			DrawCtx.Finish();
		}

		public void Wait(float time = -1) {
			cmdQueue.Enqueue(new WaitCmd{ time = time });
		}
		void Wait(WaitCmd cmd) {
			if (cmd.time < 0) {  // wait for all animation to finish
				if (lastEsEndTime > waitEndTime) waitEndTime = lastEsEndTime;
			} else {
				float endTime = time + cmd.time;
				if (endTime > waitEndTime) waitEndTime = endTime;
			}
		}

		public void AddImg(int id, int imgId) {
			if (Res.HasSpriteMeta(imgId)) {
				cmdQueue.Enqueue(new AddImgCmd{ id = id, imgId = imgId });
			} else {
				Debug.LogWarningFormat("gpc.addImg: img {0} does not exist", imgId);
			}
		}
		void AddImg(AddImgCmd cmd) {
			Lst.Push(spriteNodeLst);
			var node = (TpSpriteNode *)Lst.Last(spriteNodeLst);
			TpSpriteNode.Init(node, Res.GetSpriteMeta(cmd.imgId));

			PtrLst.Push(spriteNodePtrLst, node);
			nodeIdxDict.Add(cmd.id, spriteNodeLst->count - 1);
		}

		public void RmImg(int id) {
			cmdQueue.Enqueue(new RmImgCmd{ id = id });
		}
		void RmImg(RmImgCmd cmd) {
			if (cmd.id < 0) {
				nodeIdxDict.Clear();
				PtrLst.Clear(spriteNodePtrLst);
				Lst.Clear(spriteNodeLst);
			} else {
				var idx = nodeIdxDict[cmd.id];
				nodeIdxDict.Remove(cmd.id);
				PtrLst.Remove(spriteNodePtrLst, Lst.Get(spriteNodeLst, idx));
				Lst.RemoveAt(spriteNodeLst, idx);
			}
		}

		public void SetImgAttr(int id, int imgAttrId, params object[] args) {
			cmdQueue.Enqueue(new SetImgAttrCmd{ id = id, imgAttrId = imgAttrId, args = args });
		}
		void SetImgAttr(SetImgAttrCmd cmd) {
			var img = (TpSpriteNode *)Lst.Get(spriteNodeLst, nodeIdxDict[cmd.id]);
			var args = cmd.args;
			switch (cmd.imgAttrId) {
				case ImgAttr.Interactable: TpSpriteNode.SetInteractable(img, (bool)args[0]); break;
				case ImgAttr.Position:     TpSpriteNode.SetPosition(img, (float)args[0], (float)args[1], (float)args[2]); needDepthSort = true; break;
				case ImgAttr.Rotation:     TpSpriteNode.SetRotation(img, (float)args[0]); break;
				case ImgAttr.Scale:        TpSpriteNode.SetScale(img, (float)args[0], (float)args[1]); break;
				case ImgAttr.Alpha:        TpSpriteNode.SetAlpha(img, (float)args[0]); break;
				case ImgAttr.Tint:         TpSpriteNode.SetTint(img, (float)args[0], (float)args[1], (float)args[2]); break;
				// case ImgAttrType.ImgId:        TpSpriteNode.SetColor(img, ()args[0], ()args[1], ()args[2]); break;
			}
		}

		public void SetImgAttrEased(int id, int imgAttrId, float duration, int esType, params object[] args) {
			cmdQueue.Enqueue(new SetImgAttrEasedCmd{ id = id, imgAttrId = imgAttrId, duration = duration, esType = esType, args = args });
		}
		void SetImgAttrEased(SetImgAttrEasedCmd cmd) {
			float endTime = time + cmd.duration;
			if (endTime > lastEsEndTime) lastEsEndTime = endTime;

			var img = (TpSpriteNode *)Lst.Get(spriteNodeLst, nodeIdxDict[cmd.id]);
			var args = cmd.args;
			switch (cmd.imgAttrId) {
				case ImgAttr.Position: esJobList.AddLast(new EsSetPositionJob{ node = img, duration = cmd.duration, esType = cmd.esType, x = img->pos[0],   dx = (float)args[0] - img->pos[0], y = img->pos[1], dy = (float)args[1] - img->pos[1], z = img->pos[2], dz = (float)args[2] - img->pos[2] }); break;
				case ImgAttr.Rotation: esJobList.AddLast(new EsSetRotationJob{ node = img, duration = cmd.duration, esType = cmd.esType, r = img->rot,      dr = (float)args[0] - img->rot }); break;
				case ImgAttr.Scale:    esJobList.AddLast(new EsSetScaleJob{    node = img, duration = cmd.duration, esType = cmd.esType, x = img->scl[0],   dx = (float)args[0] - img->scl[0], y = img->scl[1], dy = (float)args[1] - img->scl[1] }); break;
				case ImgAttr.Alpha:    esJobList.AddLast(new EsSetAlphaJob{    node = img, duration = cmd.duration, esType = cmd.esType, a = img->color[3], da = (float)args[0] - img->color[3] }); break;
				case ImgAttr.Tint:     esJobList.AddLast(new EsSetTintJob{     node = img, duration = cmd.duration, esType = cmd.esType, r = img->color[0], dr = (float)args[0] - img->color[0], g = img->color[1], dg = (float)args[1] - img->color[1], b = img->color[2], db = (float)args[2] - img->color[2] }); break;
			}
		}

		public void SetCamAttr(int camAttrId, params object[] args) {
			cmdQueue.Enqueue(new SetCamAttrCmd{ camAttrId = camAttrId, args = args });
		}
		void SetCamAttr(SetCamAttrCmd cmd) {
		}

		public void SetCamAttrEased(int camAttrId, float duration, int esType, params object[] args) {
			cmdQueue.Enqueue(new SetCamAttrEasedCmd{ camAttrId = camAttrId, duration = duration, esType = esType, args = args });
		}
		void SetCamAttrEased(SetCamAttrEasedCmd cmd) {
		}
	}
}