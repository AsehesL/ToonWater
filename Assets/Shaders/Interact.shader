Shader "Hidden/Interact"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		zwrite off
		cull off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture;

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 proj : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.proj = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.proj.z);
				return o;
			}
			
			sampler2D _MainTex;

			half4 frag (v2f i) : SV_Target
			{
				half deltaDp = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.proj).r) - i.proj.z;
				// just invert the colors
				//col.rgb = 1 - col.rgb;
				clip(deltaDp);
				return deltaDp;
			}
			ENDCG
		}
	}
}
