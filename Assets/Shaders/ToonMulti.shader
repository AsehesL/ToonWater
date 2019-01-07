Shader "Custom/ToonMulti"
{
	Properties
	{
		_Color ("Color", color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_MultiTex ("MultiTex", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags { "LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv_Multi : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				float4 worldPos : TEXCOORD3;
				UNITY_FOG_COORDS(4)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Ramp;

			fixed4 _Color;

			sampler2D _MultiTex;
			float4 _MultiTex_ST;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = mul(UNITY_MATRIX_VP, o.worldPos);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_Multi = TRANSFORM_TEX(v.texcoord, _MultiTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				float3 litDir = normalize(UnityWorldSpaceLightDir(i.worldPos.xyz));
				float ndl = max(0, dot(i.worldNormal, litDir));

				fixed4 col = tex2D(_MainTex, i.uv)*_Color;

				col.rgb *= UNITY_LIGHTMODEL_AMBIENT.rgb + tex2D(_Ramp, half2(ndl, 0));

				col.rgb *= tex2D(_MultiTex, i.uv_Multi).rgb;

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
	fallback "Diffuse"
}
