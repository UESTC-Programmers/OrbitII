﻿Shader "Hidden/BuildingLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightCenter("Light Center",Range(0,1)) = 0 // 0.5是边界, 大于0.5减淡...
		_LightWidth("Light Width",Range(0.01,0.5)) = 0.01
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _LightCenter;
			float _LightWidth;

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
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float len = abs(_LightCenter - length(i.uv-fixed2(0.5,0.5)));
				col.a *= clamp(_LightWidth - len,0,1)/_LightWidth*0.5;
				return col;
			}
			ENDCG
		}
	}
}
