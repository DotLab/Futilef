using System.Collections.Generic;

using UnityEngine;

namespace Futilef {
	public class FResLevel {
		public float maxLength;
		public float displayScale, resourceScale;
		public string resourceSuffix;

		public FResLevel(float maxLength, float displayScale, float resourceScale, string resourceSuffix) {
			this.maxLength = maxLength;
			this.displayScale = displayScale;
			this.resourceScale = resourceScale;
			this.resourceSuffix = resourceSuffix;
		}
	}

	public class FConfig {
		public readonly List<FResLevel> resLevels = new List<FResLevel>();

		public Vector2 origin = new Vector2(0.5f, 0.5f);

		public int targetFrameRate = 60;

		public ScreenOrientation singleOrientation = ScreenOrientation.Unknown;

		public bool supportsLandscapeLeft, supportsLandscapeRight, supportsPortrait, supportsPortraitUpsideDown;

		public Color backgroundColor = Color.black;

		public bool shouldLerpToNearestResolutionLevel = true;

		public FConfig(bool supportsLandscapeLeft, bool supportsLandscapeRight, bool supportsPortrait, bool supportsPortraitUpsideDown) {
			this.supportsLandscapeLeft = supportsLandscapeLeft;
			this.supportsLandscapeRight = supportsLandscapeRight;
			this.supportsPortrait = supportsPortrait;
			this.supportsPortraitUpsideDown = supportsPortraitUpsideDown;
		}

		public void AddResLevel(FResLevel resLevel) {
			resLevels.Add(resLevel);
			resLevels.Sort((a, b) => a.maxLength.CompareTo(b.maxLength));
		}
	}
}

