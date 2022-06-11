Shader "Custom/Metaball"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Metaball Color", Color) = (0, 0, 1, 1)
		_Color2 ("Background", Color) = (0, 0, 1, 1)
		_C0 ("Center 1", Vector) = (0.5, 0.5, 0.0)
		_C1 ("Center 2", Vector) = (0.5, 0.5, 0.0)
		// _ArrayLen ("Array Length", int) = 0

	}
	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _Color;
			float4 _Color2;
			
			float4 _C0;
			float4 _C1;
			
			uniform int _Blobs_Length = 1;
 			uniform float3 _Blobs [1000]; // (x, y) = position ; z = radius

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float mx = 0;
				for (int j = 0; j < _Blobs_Length; j++)
				{
					// mx +=_Blobs[j].x;
					mx += pow(_Blobs[j].z, 2) / (pow(i.uv.x - _Blobs[j].x, 2) + pow(i.uv.y - _Blobs[j].y, 2));
				}

				float n0 = pow(_C0.z, 2) / (pow(i.uv.x - _C0.x, 2) + pow(i.uv.y - _C0.y, 2));
				float n1 = pow(_C1.z, 2) / (pow(i.uv.x - _C1.x, 2) + pow(i.uv.y - _C1.y, 2));
				mx = mx + n0 + n1;
				
				if (mx > 1)
					return _Color;
				else 
					return _Color2;
			}


			
			ENDCG
		}
	}
}
