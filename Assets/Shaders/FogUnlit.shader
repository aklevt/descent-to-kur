Shader "Custom/Fog2D"
{
    Properties
    {
        [MainColor] _Color("Fog Color", Color) = (0.2, 0.2, 0.3, 0.5)
        _ScrollX("Scroll X", Float) = 0.05
        _ScrollY("Scroll Y", Float) = 0.02
        _Scale("Noise Scale", Float) = 3.0
        _Alpha("Opacity", Range(0, 1)) = 0.7
        _Detail("Detail", Range(1, 4)) = 2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            half4 _Color;
            half _ScrollX;
            half _ScrollY;
            half _Scale;
            half _Alpha;
            half _Detail;


            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            float perlin(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                float v0 = lerp(a, b, u.x);
                float v1 = lerp(c, d, u.x);
                return lerp(v0, v1, u.y);
            }


            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 1.0;
                float frequency = 1.0;
                float maxValue = 0.0;

                for (int i = 0; i < 4; i++)
                {
                    value += amplitude * perlin(p * frequency);
                    maxValue += amplitude;
                    amplitude *= 0.5;
                    frequency *= 2.0;
                }

                return value / maxValue;
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv1 = i.uv * _Scale + float2(_ScrollX, _ScrollY) * _Time.y;
                float2 uv2 = i.uv * _Scale * 0.5 - float2(_ScrollX * 0.6, _ScrollY * 0.6) * _Time.y;


                float noise1 = fbm(uv1);
                float noise2 = fbm(uv2 * 1.5);


                float combined = noise1 * 0.7 + noise2 * 0.3;


                combined = pow(combined, 1.5);


                half4 col = _Color;
                col.a *= combined * _Alpha;

                return col;
            }
            ENDHLSL
        }
    }
}