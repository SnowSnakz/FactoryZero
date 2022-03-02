Shader "Unlit/ScatterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthFactor("Depth Factor", float) = 1.0
        _DepthPow("Depth Pow", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        GrabPass
        {
            "_Behind"
        }

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
                float4 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Behind;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            float4 _MainTex_ST;
            float _DepthFactor;
            fixed _DepthPow;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.screenPos.z);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture

                float4 f = tex2D(_MainTex, i.uv);

                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
                float depth = sceneZ - i.screenPos.z + (1 - f.a);
                depth = saturate((abs(pow(depth, _DepthPow))) / _DepthFactor);;

                float4 bc = tex2D(_Behind, i.uv);
                float cp = (bc.r + bc.g + bc.b) * 0.33;
                fixed4 col = lerp(f, bc, 1 - (depth * (1 - cp)));
                col.a = 1;

                return col;
            }
            ENDCG
        }
    }
}
