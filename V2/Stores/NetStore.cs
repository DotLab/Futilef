﻿using System.Net;

namespace Futilef.V2 {
	public class NetStore : IStore<byte[]> {
		public byte[] Get(string url) {
			var req = new WebClient();
			return req.DownloadData(url);
		}
	}
}

