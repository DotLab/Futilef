// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading;

namespace Futilef.V2 {
	public class ObjectUsage<T> : IDisposable {
		public T obj;
		public int index;

		public long frameId;

		public Action<ObjectUsage<T>, UsageType> finish;

		public UsageType usage;

		public ObjectUsage(int index, Action<ObjectUsage<T>, UsageType> finish) {
			this.index = index;
			this.finish = finish;
			usage = UsageType.None;
		}

		public void Dispose() {
			if (finish != null) finish.Invoke(this, usage);
		}
	}

	public enum UsageType {
		None,
		Read,
		Write
	}

	/// <summary>
	/// Handles triple-buffering of any object type.
	/// Thread safety assumes at most one writer and one reader.
	/// </summary>
	public class TripleBuffer<T> {
		readonly ObjectUsage<T>[] buffers = new ObjectUsage<T>[3];

		int read;
		int write;
		int lastWrite = -1;

		long currentFrame;

		readonly Action<ObjectUsage<T>, UsageType> finishAction;

		public TripleBuffer() {
			//caching the delegate means we only have to allocate it once, rather than once per created buffer.
			finishAction = Finish;

			for (int i = 0; i < 3; i += 1) buffers[i] = new ObjectUsage<T>(i, finishAction);
		}

		public ObjectUsage<T> Get(UsageType usage) {
			switch (usage) {
			case UsageType.Write:
				lock (buffers) {
					while (buffers[write].usage == UsageType.Read || write == lastWrite) write = (write + 1) % 3;
				}

				buffers[write].usage = UsageType.Write;
				buffers[write].frameId = Interlocked.Increment(ref currentFrame);

				return buffers[write];
			case UsageType.Read:
				if (lastWrite < 0) return null;

				lock (buffers) {
					read = lastWrite;
					buffers[read].usage = UsageType.Read;
				}

				return buffers[read];
			}

			return null;
		}

		void Finish(ObjectUsage<T> obj, UsageType type) {
			switch (type) {
			case UsageType.Read:
				lock (buffers) {
					buffers[read].usage = UsageType.None;
				}
				break;
			case UsageType.Write:
				lock (buffers) {
					buffers[write].usage = UsageType.None;
					lastWrite = write;
				}
				break;
			}
		}
	}
}
