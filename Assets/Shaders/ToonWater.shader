Shader "Unlit/ToonWater"
{
	Properties
	{
		_ReflRamp ("ReflRamp", 2D) = "white" {}
		_WaterRamp ("WaterRamp", 2D) = "white" {}
		_Foam ("Foam", 2D) = "white" {}
		_InteractFoam ("InteractFoam", 2D) = "black" {}
		_Speed ("Speed(X, Y)", vector) = (0, 0, 0, 0)
		_FoamCutoff ("FoamCutoff", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				float4 proj : TEXCOORD3;
				UNITY_FOG_COORDS(4)
				float4 vertex : SV_POSITION;
			};

			sampler2D _ReflRamp;
			sampler2D _WaterRamp;
			sampler2D _Foam;
			sampler2D _InteractFoam;
			float4 _Foam_ST;
			sampler2D _CameraDepthTexture;

			half4 _Speed;
			half _FoamCutoff;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = UnityObjectToWorldNormal(v.normal).xyz;
				o.proj = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.proj.z);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.proj).r) - i.proj.z;
				float interact = tex2D(_InteractFoam, i.uv).r;

				half3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				half rdn = max(0, dot(viewDir, i.worldNormal));

				fixed4 refcol = tex2D(_ReflRamp, half2(rdn, 0));
				fixed4 wcol = tex2D(_WaterRamp, half2(depth, 0))*0.5 + refcol*0.5;

				float depthParam = pow(1 - depth, 3)+interact*2;

				half2 uvfoam = TRANSFORM_TEX(i.uv, _Foam);

				fixed foam = (tex2D(_Foam, uvfoam + half2(_Speed.x, _Speed.y)*_Time.y) + tex2D(_Foam, half2(uvfoam.x + _Speed.x*_Time.y, 1 - uvfoam.y + _Speed.y*_Time.y))).r*0.5*depthParam + depthParam*0.35;
				//foam *= tex2D(_InteractFoam, i.uv).r*0.5;
				foam = saturate((foam - _FoamCutoff)*30);

				wcol = wcol * (1 - foam) + foam;

				UNITY_APPLY_FOG(i.fogCoord, wcol);
				return wcol;
			}
			ENDCG
		}
	}
}
