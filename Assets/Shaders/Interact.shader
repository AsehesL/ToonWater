Shader "Hidden/Interact"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		zwrite off
		Pass
		{
			cull front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _DepthTexture;

			struct v2f
			{
				float4 proj : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.proj = ComputeScreenPos(o.vertex);
				o.proj.z = COMPUTE_DEPTH_01;
				return o;
			}
			
			sampler2D _MainTex;

			half4 frag (v2f i) : SV_Target
			{
				half deltaDp = tex2Dproj(_DepthTexture, i.proj).r - i.proj.z;

				clip(-deltaDp);

				return 1;
			}
			ENDCG
		}
		Pass
		{
			cull back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _DepthTexture;

			struct v2f
			{
				float4 proj : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.proj = ComputeScreenPos(o.vertex);
				o.proj.z = COMPUTE_DEPTH_01;
				return o;
			}

			sampler2D _MainTex;

			half4 frag(v2f i) : SV_Target
			{
				half deltaDp = tex2Dproj(_DepthTexture, i.proj).r - i.proj.z;
				
				clip(-deltaDp);

				return 0;
			}
			ENDCG
		}
	}
}
