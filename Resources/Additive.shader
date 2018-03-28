Shader "Futilef/Additive" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
	}

	SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		
		// Alpha Blend
		// Blend SrcAlpha OneMinusSrcAlpha
		// Additive
		Blend SrcAlpha One

		Pass {
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment SpriteFrag

			#include "UnityCG.cginc"

			fixed4 _Color;

			struct appdata_t {
				float4 vertex   : POSITION;
				float4 color	: COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex   : SV_POSITION;
				fixed4 color	: COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f SpriteVert(appdata_t IN) {
				v2f OUT;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 SpriteFrag(v2f IN) : SV_Target {
				return tex2D(_MainTex, IN.texcoord) * IN.color;
			}
		ENDCG
		}
	}
}
