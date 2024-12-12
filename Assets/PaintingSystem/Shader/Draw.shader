Shader "Hidden/Draw"
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

            float4 _MousePosition;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _PositionTex;
            sampler2D _PaintTex;
            float4 _Color;

            //rotate UV function from
            float2 rotateUV(float2 uv, float amount){
                float sinX = sin ( amount );
			    float cosX = cos ( amount );
			    float sinY = sin ( amount );
			    float2x2 rotationMatrix = float2x2( cosX, -sinX, sinY, cosX);
                return mul ( uv, rotationMatrix );
            }

            //Hashing for psuedo random - taken from Hash without sine https://www.shadertoy.com/view/4djSRW
            float hash11(float p1)
            {
                float p = frac(p1 * .1031);
                p *= p + 33.33;
                p *= p + p;
                return frac(p);
            }

            float _BrushTexScale;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                fixed4 pos = tex2D(_PositionTex, i.uv);
                
                //Screen space uv set up
                float2 ratio = _ScreenParams.xy/_ScreenParams.y;
                float2 uv = (pos.xy*ratio) - ((_MousePosition.xy/_ScreenParams.xy)*ratio);
                
                float rand = hash11(round((_Time.y + _MousePosition.x + _MousePosition.y) * 100));
                uv = rotateUV(uv, atan2(_MousePosition.w, _MousePosition.z) - atan2(1,0));
                //Magic number - unsure why this works, theory this is linked to screen scaling?
                uv *= 400/(_BrushTexScale);
                uv+= 0.5;
                
                //make sure stays in bounds of screen
                uv = (uv.x<1 && uv.x>0 && uv.y<1 && uv.y>0) ? uv : float2(-1,-1);
                
                float4 paint = tex2D(_PaintTex, uv);

                //lerp function for painting - need to refine so colour is more consistant
                float3 f = lerp(col.rgb , _Color.rgb, _Color.a * unity_DeltaTime.x * 20 * (pos.z>0) * paint.a);
                return float4(f,1);
            }
            ENDCG
        }
    }
}
