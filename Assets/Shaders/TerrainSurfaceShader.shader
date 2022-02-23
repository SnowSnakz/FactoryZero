Shader "Custom/TerrainSurfaceShader"
{
    Properties
    {
        _TopColorLight ("Biome Colors", 2D) = "white" {}
        _SideColorLight ("X1 (RGB)", 2D) = "white" {}
        _BiomeColors ("X2 (RGB)", 2D) = "white" {}
        _DarkColor ("Dark Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert vertex:vert fullforwardshadows nolightmap

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _TopColorLight;
        sampler2D _SideColorLight;
        sampler2D _BiomeColors;

        half3 _DarkColor;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;

            float4 TopColorLight;
            float4 SideColorLight;
            float4 BiomeColors;
        };

        #include "noiseSimplex.cginc"

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            o.TopColorLight = v.texcoord;
            o.SideColorLight = v.texcoord1;
            o.BiomeColors = v.texcoord2;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Color Noise
            float ns = abs((snoise(IN.worldPos) * 0.25 + snoise(IN.worldPos * 2.5) * 0.5 + (snoise(IN.worldPos * 5) * 0.5)) * 0.33);

            // Biome Color
            float4 biomeColor = tex2D(_BiomeColors, IN.BiomeColors.xy);

            // Top Color
            float3 topColor = lerp(IN.TopColorLight.rgb, _DarkColor, ns * 0.2);
            topColor = lerp(topColor, topColor * biomeColor, IN.BiomeColors.z);

            // Side Color
            float3 sideColor = lerp(IN.SideColorLight.rgb, _DarkColor, ns * 0.2);
            sideColor = lerp(sideColor, sideColor * biomeColor, IN.BiomeColors.w);

            // Top/Side Blending
            float topMix = floor((clamp(IN.worldNormal.y, 0, 1) + IN.TopColorLight.w + ns * 0.1) * IN.SideColorLight.w);

            o.Albedo = lerp(sideColor, topColor, topMix); //float3(IN.BiomeColors.xy, 0); 
        }
        ENDCG
    }
    FallBack "Diffuse"
}
