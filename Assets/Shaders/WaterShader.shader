Shader "Custom/WaterShader"
{
    Properties
    {
        _Color ("Light Color", Color) = (1,1,1,1)
        _DarkColor ("Dark Color", Color) = (1,1,1,1)
        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _WaveHeight ("Wave Height", Range(0,5)) = 0.0
        _WaveScale ("Wave Scale", Range(0,10)) = 0.0
        _WaveSpeed ("Wave Speed", Range(0,5)) = 0.0
        _FresnelExponent("Fresnel Exponent", Range(0,5)) = 0.0
        _Normal1Scale("Normal Scale 1", Range(0,5)) = 0.0
        _Normal2Scale("Normal Scale 2", Range(0,5)) = 0.0
        _Normal1Speed("Normal Speed 1", Vector) = (0.0, 0, 0, 0)
        _Normal2Speed("Normal Speed 2", Vector) = (0.0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        ZWrite Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:fade vertex:vert fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _CameraDepthTexture;
        sampler2D _MainTex;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            float3 viewDir;
            float4 screenPos;

            INTERNAL_DATA
        };

        half _Glossiness;
        half _Metallic;
        half _WaveHeight;
        half _WaveScale;
        half _WaveSpeed;
        half _FresnelExponent;

        fixed4 _Color;
        fixed4 _DarkColor;

        half _Normal1Scale;
        half _Normal2Scale;

        fixed3 _Normal1Speed;
        fixed3 _Normal2Speed;

        fixed3 _FresnelColor;

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float3 wp = mul(unity_ObjectToWorld, v.vertex);

            v.vertex.y += cos(wp.x * _WaveScale + wp.y * _WaveScale + (_Time.z * _WaveSpeed)) * _WaveHeight;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float fresnel = dot(IN.worldNormal, IN.viewDir);
            fresnel = saturate(1 - fresnel);
            fresnel = pow(fresnel, _FresnelExponent);

            float3 fresnelColor = fresnel * _FresnelColor;

            float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
            fixed4 c2 = lerp(lerp(_Color, _DarkColor, 1 - depth), float4(fresnelColor, 1), fresnel);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * c2;
            o.Albedo = c;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = clamp(pow(1 - depth + (c2.a * 0.05), 3), 0, 1);
            //o.Emission = float4(fresnelColor, 0);

            o.Normal = BlendNormals(UnpackScaleNormal(tex2D(_BumpMap, (IN.worldPos.xz + _Normal1Speed * _Time.x) * _Normal1Scale), _Normal1Speed.z), UnpackScaleNormal(tex2D(_BumpMap, (IN.worldPos.xz + _Normal2Speed * _Time.x) * _Normal2Scale), _Normal2Speed.z));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
