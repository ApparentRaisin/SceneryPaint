Shader "Hidden/Grow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            float4 _MainTex_TexelSize;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                if(col.a < 1.){
                    col = max(col, tex2D(_MainTex, i.uv + float2(1,0) * _MainTex_TexelSize.xy * 1));
                    col = max(col, tex2D(_MainTex, i.uv + float2(-1,0) * _MainTex_TexelSize.xy * 1));
                    col = max(col, tex2D(_MainTex, i.uv + float2(0,1) * _MainTex_TexelSize.xy * 1));
                    col = max(col, tex2D(_MainTex, i.uv + float2(0,-1) * _MainTex_TexelSize.xy * 1));
                    
                    col = max(col, tex2D(_MainTex, i.uv + float2(1,0) * _MainTex_TexelSize.xy * 2));
                    col = max(col, tex2D(_MainTex, i.uv + float2(-1,0) * _MainTex_TexelSize.xy * 2));
                    col = max(col, tex2D(_MainTex, i.uv + float2(0,1) * _MainTex_TexelSize.xy * 2));
                    col = max(col, tex2D(_MainTex, i.uv + float2(0,-1) * _MainTex_TexelSize.xy * 2));

                    col = max(col, tex2D(_MainTex, i.uv + float2(1,0) * _MainTex_TexelSize.xy * 3));
                    col = max(col, tex2D(_MainTex, i.uv + float2(-1,0) * _MainTex_TexelSize.xy * 3));
                    col = max(col, tex2D(_MainTex, i.uv + float2(0,1) * _MainTex_TexelSize.xy * 3));
                    col = max(col, tex2D(_MainTex, i.uv + float2(0,-1) * _MainTex_TexelSize.xy * 3));

                }
                return col;
            }
            ENDCG
        }
    }
}
