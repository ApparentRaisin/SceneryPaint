Shader "Unlit/PaintUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ClipTex ("Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", float) = 0
    }
    SubShader
    {
        //Basic cutout shader for all painted objects
        Tags { "RenderType"="TransparentCutout" }
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
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _ClipTex;
            float4 _ClipTex_ST;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _ClipTex);
                o.uv2 = v.uv2;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv2);
                fixed clipTex = tex2D(_ClipTex, i.uv).a;
                
                clip(clipTex - _Cutoff);
                return col;
            }
            ENDCG
        }

        //shadow pass needed to render to depth pass
        Pass
        {
            Tags {"LightMode" = "ShadowCaster"}

            CGPROGRAM
            #pragma vertex v2f_shadow
            #pragma fragment frag_shadow

            #pragma target 2.0
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
            
            #include "UnityCG.cginc"


            struct v2f
            {
                
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _ClipTex;
            float4 _ClipTex_ST;
            float _Cutoff;

            v2f v2f_shadow (appdata_base v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                o.uv = TRANSFORM_TEX(v.texcoord, _ClipTex);
                return o;
            }

            fixed4 frag_shadow (v2f i) : SV_Target
            {
                fixed clipTex = tex2D(_ClipTex, i.uv).a;
                
                clip(clipTex - _Cutoff);
                SHADOW_CASTER_FRAGMENT(i);
            }

            #include "UnityCG.cginc"
            ENDCG
        }

    }

}
