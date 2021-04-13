Shader "Unlit/BufferViewer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientR ("GradientR", 2D) = "white" {}
        _GradientG ("GradientG", 2D) = "white" {}
        _GradientB ("GradientB", 2D) = "white" {}
        _GradientA ("GradientA", 2D) = "white" {}

        trailMax("Trail Max", range(1,10))=1
        gradiantCurve("gradientCurve", range(1,5))=1
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

            sampler2D _MainTex;
            sampler2D _GradientR;
            sampler2D _GradientG;
            sampler2D _GradientB;            
            sampler2D _GradientA;            
            float4 _MainTex_ST;
            float trailMax;
            float gradiantCurve;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 s = tex2D(_MainTex, i.uv);
                float4 value = pow(s/trailMax,gradiantCurve);
                float4 R = tex2D(_GradientR, float2(value.r,0));
                float4 G = tex2D(_GradientG, float2(value.g,0));
                float4 B = tex2D(_GradientB, float2(value.b,0));
                float4 A = tex2D(_GradientA, float2(value.a,0));
                value=normalize(value);
                return (R*value.r+G*value.g+B*value.b+A*value.a);///(dot(value,float4(1,1,1,1)));
            }
            ENDCG
        }
    }
}
