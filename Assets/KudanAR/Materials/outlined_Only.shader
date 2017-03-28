// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Outlined/Texture" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	[Space(15)][MaterialToggle] _OutlineOnOff ("Outline On/Off", Float ) = 1
		_Color ("Main Color", Color) = (.5,.5,.5,1)
        _OutlineColor ("Outline Color", Color) = (0.31,0.56,0.71,1)
        _OutlineWidth ("Outline Width", Range(0, 1)) = 0.6
}

SubShader {
	Tags { "RenderType"="Opaque" }
	//Tags { "Queue" = "Transparent" }
	LOD 100
	Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _OutlineColor;
            uniform float _OutlineWidth;
            uniform fixed _OutlineOnOff;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float node_5425 = 0.0;
                o.pos = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz + v.vertexColor*lerp( node_5425, lerp(node_5425,0.05,_OutlineWidth), _OutlineOnOff ),1) );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                return fixed4(_OutlineColor.rgb,0);
            }
            ENDCG
        }

        Pass {
			Name "BASE"
			ZWrite On
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			Material {
				Diffuse [_Color]
				Ambient [_Color]
			}
			Lighting On
			SetTexture [_MainTex] {
				ConstantColor [_Color]
				Combine texture * constant
			}
			SetTexture [_MainTex] {
				Combine previous * primary DOUBLE
			}
		}

//	Pass {  
//		CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#pragma target 2.0
//			#pragma multi_compile_fog
//			uniform fixed _TextureOn;
//			
//			#include "UnityCG.cginc"
//
//			struct appdata_t {
//				float4 vertex : POSITION;
//				float2 texcoord : TEXCOORD0;
//			};
//
//			struct v2f {
//				float4 vertex : SV_POSITION;
//				half2 texcoord : TEXCOORD0;
//				UNITY_FOG_COORDS(1)
//			};
//
//			sampler2D _MainTex;
//			float4 _MainTex_ST;
//			
//			v2f vert (appdata_t v)
//			{
//				v2f o;
//				o.vertex = UnityObjectToClipPos(v.vertex);
//				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
//				UNITY_TRANSFER_FOG(o,o.vertex);
//				return o;
//			}
//			
//			fixed4 frag (v2f i) : SV_Target
//			{
//				fixed4 col = tex2D(_MainTex, i.texcoord);
//				UNITY_APPLY_FOG(i.fogCoord, col);
//				UNITY_OPAQUE_ALPHA(col.a);
//				return col;
//			}
//		ENDCG
//	}
}

}
