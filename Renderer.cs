using System;
using System.Collections.Generic;

namespace Futilef {
	public static class Renderer {
		public const int BaseRenderQueue = 3000;

		static List<RenderLayer> _activeLayers = new List<RenderLayer>();
		static List<RenderLayer> _previouslyAcitiveLayers = new List<RenderLayer>();
		static readonly List<RenderLayer> _inactiveLayers = new List<RenderLayer>();

		static int _currentRenderQueue;
		static RenderLayer _currentLayer;

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

		public static RenderLayer GetRenderLayer(Atlas atlas, Shader shader, PrimitiveType type) {
			if (_currentLayer == null || atlas != _currentLayer.atlas || shader != _currentLayer.shader || type != _currentLayer.type) {
				if (_currentLayer != null) _currentLayer.Close();
				_currentLayer = CreateRenderLayer(atlas, shader, type);
				_currentLayer.Open(_currentRenderQueue++);
			}

			return _currentLayer;
		}

		static RenderLayer CreateRenderLayer(Atlas atlas, Shader shader, PrimitiveType type) {
			// If there is a previouslyActiveLayer that mathes
			for (int i = 0; i < _previouslyAcitiveLayers.Count; i++) {
				var layer = _previouslyAcitiveLayers[i];
				if (layer.atlas == atlas && layer.shader == shader) {
					_previouslyAcitiveLayers.RemoveAt(i);
					_activeLayers.Add(layer);
					return layer;
				}
			}

			// If there is a inactiveLayer that matches
			for (int i = 0; i < _inactiveLayers.Count; i++) {
				var layer = _inactiveLayers[i];
				if (layer.atlas == atlas && layer.shader == shader) {
					_inactiveLayers.RemoveAt(i);
					layer.Activate();
					_activeLayers.Add(layer);
					return layer;
				}
			}

			// Create a new one
			RenderLayer newLayer = null;
			if (type == PrimitiveType.Triangle) newLayer = new RenderLayer.Triangle(atlas, shader);
			else if (type == PrimitiveType.Quad) newLayer = new RenderLayer.Quad(atlas, shader);
			else throw new ArgumentOutOfRangeException();
			return newLayer;
		}
	}
}

