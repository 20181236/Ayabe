Shader "Custom/RobotMetallicShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Use the built-in Standard lighting model
        #pragma surface surf Standard fullforwardshadows

        // The name of the function for the surface shader is "surf"
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        half _Glossiness;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo is the base color of the object.
            // We use the texture and multiply it by the color property.
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Assign the metallic and smoothness properties
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            // Keep the alpha for transparency, if any.
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}