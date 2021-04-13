Shader "Unlit/MapSHader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        xx ("xx", range(10,100)) = 50
        // k ("k",float) = 0.4
        // c ("c",float) = 40
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            uniform float4 _MainTex_TexelSize;
            float xx ;
            // float k;
            // float c;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float4 fetch( float2 uv)
            {
                uv = (floor(uv * _MainTex_TexelSize.zw) + 0.5) * _MainTex_TexelSize.xy;
                float4 col = tex2Dlod(_MainTex, float4(uv, 0, 0));      
                return col;          

            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 v = 0;
                xx = 1/(xx*xx*xx*xx);
                float2 center = (floor(i.uv * _MainTex_TexelSize.zw) + 0.5) * _MainTex_TexelSize.xy;
                int size = 0;
                for (int x = -size; x <=size; x++)
                {
                    for (int y = -size; y <=size; y++)
                    {              
                        float2 diff = i.uv - (center + float2(x,y)*_MainTex_TexelSize.xy);
                        float density = xx/dot(diff,diff);
                        v += density* fetch(center + float2(x,y)*_MainTex_TexelSize.xy);
                    }
                }
                
                return lerp(float4(v.xzy,1), float4(0.4,0.2,0.3,1), 1-saturate(v.x+v.y));
            }
            ENDCG
        }
    }
}
