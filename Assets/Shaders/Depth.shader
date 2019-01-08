Shader "Hidden/Depth"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"


			struct v2f
			{
				float depth : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.depth = COMPUTE_DEPTH_01;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(i.depth, i.depth, i.depth, 1.0);
			}
			ENDCG
		}
	}
}