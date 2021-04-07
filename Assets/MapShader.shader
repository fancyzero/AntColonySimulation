Shader "Unlit/MapShader"
{
    Properties
    {
        
        NumGrids ("Num Grids",float)=0
        FoodMark ("Food Mark",2D)="Black" {}
        HomeMark ("Home Mark",2D)="Black" {}
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float NumGrids;
            sampler2D FoodMark;
            sampler2D HomeMark;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                // apply fog
                float food = tex2D(FoodMark, i.uv).x;
                float home = tex2D(HomeMark, i.uv).x;
                
                return float4(food,(1-saturate(food+home))*0.5,home,1);
            }
            ENDCG
        }
    }
}
