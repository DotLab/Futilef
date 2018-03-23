using System;
using System.Collections.Generic;

using UnityEngine;

namespace Futilef.Rendering {
	public static class FRenderer {
		public const int BaseRenderQueue = 3000;

		static List<FRenderLayer> _activeLayers = new List<FRenderLayer>();
		static List<FRenderLayer> _previouslyAcitiveLayers = new List<FRenderLayer>();
		static readonly List<FRenderLayer> _inactiveLayers = new List<FRenderLayer>();

		static int _currentRenderQueue;
		static FRenderLayer _currentLayer;

		public static void StartRender() {
			_currentRenderQueue = BaseRenderQueue;
			_currentLayer = null;
		}

		public static void EndRender() {
			foreach (var layer in _previouslyAcitiveLayers) layer.Deactivate();
			_inactiveLayers.AddRange(_previouslyAcitiveLayers);
			_previouslyAcitiveLayers.Clear();

			// Same as _preiouslyActiveLayers.AddRange(_activeLayers); _activeLayers.Clear();
			var swap = _previouslyAcitiveLayers;
			_previouslyAcitiveLayers = _activeLayers;
			_activeLayers = swap;

			if (_currentLayer != null) {
				_currentLayer.Close();
				_currentLayer = null;
			}
		}

		public static FRenderLayer GetRenderLayer(Texture2D texture, Shader shader, PrimitiveType type) {
			if (_currentLayer == null || texture != _currentLayer.texture || shader != _currentLayer.shader || type != _currentLayer.type) {
				if (_currentLayer != null) _currentLayer.Close();
				_currentLayer = CreateRenderLayer(texture, shader, type);
				_currentLayer.Open(_currentRenderQueue++);
			}

			return _currentLayer;
		}

		static FRenderLayer CreateRenderLayer(Texture2D texture, Shader shader, PrimitiveType type) {
			// If there is a previouslyActiveLayer that mathes
			for (int i = 0; i < _previouslyAcitiveLayers.Count; i++) {
				var layer = _previouslyAcitiveLayers[i];
				if (layer.texture == texture && layer.shader == shader) {
					_previouslyAcitiveLayers.RemoveAt(i);
					_activeLayers.Add(layer);
					return layer;
				}
			}

			// If there is a inactiveLayer that matches
			for (int i = 0; i < _inactiveLayers.Count; i++) {
				var layer = _inactiveLayers[i];
				if (layer.texture == texture && layer.shader == shader) {
					_inactiveLayers.RemoveAt(i);
					layer.Activate();
					_activeLayers.Add(layer);
					return layer;
				}
			}

			// Create a new one
			FRenderLayer newLayer = null;
			if (type == PrimitiveType.Triangle) newLayer = new FRenderLayer.Triangle(texture, shader);
			else if (type == PrimitiveType.Quad) newLayer = new FRenderLayer.Quad(texture, shader);
			else throw new ArgumentOutOfRangeException();
			return newLayer;
		}
	}
}

