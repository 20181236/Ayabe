Shader "Custom/OutlineShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Back

        // Pass 1: Outline (slightly scaled mesh)
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }

            Cull Front
            ZWrite On
            ZTest Less

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                v.vertex.xyz += norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // Pass 2: Regular Surface Shader
        CGPROGRAM
        #pragma surface surf Standard

        fixed4 _Color;

        struct Input
        {
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
