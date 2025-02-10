Shader "Hidden/OutlineReplacement"
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
                float3 viewDir : TEXCOORD1;
            };

            float4x4 _InvProjectionMatrix;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.viewDir = mul(_InvProjectionMatrix, float4(o.vertex.xyz,1)).xyz;
                o.viewDir.y *= _ProjectionParams.x;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _NormalsTex;
            sampler2D _CameraDepthTexture;
            

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                //Sample Normals and depth texture                
                float4 nUp = tex2D(_NormalsTex, i.uv + float2(1,1) *_MainTex_TexelSize.xy * 1.5);
                float4 nDown = tex2D(_NormalsTex, i.uv + float2(-1,-1) *_MainTex_TexelSize.xy * 1.5);
                float4 nLeft = tex2D(_NormalsTex, i.uv + float2(1,-1) *_MainTex_TexelSize.xy * 1.5);
                float4 nRight = tex2D(_NormalsTex, i.uv + float2(-1,1) *_MainTex_TexelSize.xy * 1.5);

                //calculate grazing angle threshold for depth
                float NdotV = (dot(nUp.xyz*2-1, -i.viewDir)) ;

                //normal outline
                float nO = 1-smoothstep(0.,1.,sqrt(
                    pow(dot(nUp.xyz*2-1,nDown.xyz*2-1),2) + 
                    pow(dot(nLeft.xyz*2-1,nRight.xyz*2-1),2)
                    ));

                //depth outline
                float dO = pow(sqrt(pow(((nUp.a) - (nDown.a)),2) + 
                pow(((nLeft.a) - (nRight.a)),2)) * 100 * NdotV,1.5);

                return col - (clamp((dO+nO),0,1) > 0.2);
            }
            ENDCG
        }
    }
}
