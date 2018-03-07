using System.Collections.Generic;

namespace Futilef {
	public class FFacetType {
		public static FFacetType Default, Quad, Triangle;

		static readonly List<FFacetType> _facetTypes = new List<FFacetType>();

		public int index;
		public string name;

		public int initialAmount, expansionAmount, maxEmptyAmout;

		public System.Action createRenderLayerHandler;

		public FFacetType(int index, string name, int initialAmount, int expansionAmount, int maxEmptyAmout, System.Action createRenderLayerHandler) {
			this.index = index;
			this.name = name;
		
			this.initialAmount = initialAmount;
			this.expansionAmount = expansionAmount;
			this.maxEmptyAmout = maxEmptyAmout;

			this.createRenderLayerHandler = createRenderLayerHandler;
		}

		public static void Init() {
			Default = Quad = Create("Quad", 10, 10, 60, () => {
			});
			Triangle = Create("Triangle", 16, 16, 64, () => {
			});
		}

		public static FFacetType Create(string name, int initialAmount, int expansionAmount, int maxEmptyAmout, System.Action createRenderLayerHandler) {
			foreach (var ft in _facetTypes) if (ft.name.Equals(name)) return ft;

			var facetType = new FFacetType(_facetTypes.Count, name, initialAmount, expansionAmount, maxEmptyAmout, createRenderLayerHandler);
			_facetTypes.Add(facetType);
			return facetType;
		}
	}
}

