Shader "Custom/UI_Ultimate_Optimized"
{
    Properties
    {
        // Основные
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)

        // Outline
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02

        // Форма
        _ShapeType ("Shape Type", Float) = 0          // 0=Rect, 1=Circle, 2=RoundedRect, 3=Diamond
        _CornerRadius ("Corner Radius", Range(0, 0.5)) = 0.1

        // Padding (внутренние отступы) – сдвигает края формы внутрь
        _Padding ("Padding (X, Y)", Vector) = (0,0,0,0)

        // Градиент
        [Toggle(_GRADIENT_ENABLE)] _GradientEnable ("Enable Gradient", Float) = 0
        _GradientColorTop ("Gradient Top", Color) = (1,1,1,1)
        _GradientColorBottom ("Gradient Bottom", Color) = (0,0,0,1)
        _GradientDirection ("Gradient Direction", Float) = 0
        _GradientBlend ("Gradient Blend", Range(0,1)) = 0.5

        // Rim
        [Toggle(_RIM_ENABLE)] _RimEnable ("Enable Rim", Float) = 0
        _RimColor ("Rim Color", Color) = (1,1,0,1)
        _RimWidth ("Rim Width", Range(0, 0.5)) = 0.1
        _RimPower ("Rim Softness", Range(1, 5)) = 2

        // Highlight
        [Toggle(_HIGHLIGHT_ENABLE)] _HighlightEnable ("Enable Highlight", Float) = 0
        _HighlightColor ("Highlight Color", Color) = (1,1,1,0.5)
        _HighlightWidth ("Highlight Width", Range(0.01, 0.5)) = 0.15
        _HighlightPosition ("Highlight Position", Range(-1, 2)) = 0.0
        _HighlightAngle ("Highlight Angle", Range(0, 360)) = 45
        _HighlightBlend ("Highlight Blend", Range(0,1)) = 0.7
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "UI Ultimate"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing

            #pragma shader_feature _GRADIENT_ENABLE
            #pragma shader_feature _RIM_ENABLE
            #pragma shader_feature _HIGHLIGHT_ENABLE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half4 _OutlineColor;
                float _OutlineWidth;
                float _ShapeType;
                float _CornerRadius;
                float2 _Padding;                // (отступ по X, отступ по Y)
                half4 _GradientColorTop;
                half4 _GradientColorBottom;
                float _GradientDirection;
                float _GradientBlend;
                half4 _RimColor;
                float _RimWidth;
                float _RimPower;
                half4 _HighlightColor;
                float _HighlightWidth;
                float _HighlightPosition;
                float _HighlightAngle;
                float _HighlightBlend;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Вычисление альфы формы для заданных UV
            float GetShapeAlpha(float2 uv, float paddingX, float paddingY, float shapeType, float cornerRadius)
            {
                float2 center = float2(0.5, 0.5);
                float2 size = float2(0.5 - paddingX, 0.5 - paddingY);

                // Прямоугольник (без скругления)
                if (shapeType < 0.5) // 0
                {
                    float2 d = abs(uv - center) - size;
                    return 1.0 - saturate(max(d.x, d.y) * 20.0); // сглаживание
                }
                // Круг
                else if (shapeType < 1.5) // 1
                {
                    float radius = min(size.x, size.y);
                    float dist = distance(uv, center);
                    return 1.0 - saturate((dist - radius) * 20.0);
                }
                // Скругленный прямоугольник
                else if (shapeType < 2.5) // 2
                {
                    float r = min(cornerRadius, min(size.x, size.y));
                    float2 d = abs(uv - center) - (size - r);
                    float dist = length(max(d, 0.0)) - r;
                    return 1.0 - saturate(dist * 20.0);
                }
                // Ромб
                else // 3
                {
                    float2 d = abs(uv - center);
                    float dist = d.x / max(size.x, 0.001) + d.y / max(size.y, 0.001);
                    return 1.0 - saturate((dist - 1.0) * 20.0);
                }
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Получаем параметры
                float shapeType = _ShapeType;
                float padX = _Padding.x;
                float padY = _Padding.y;
                float corner = _CornerRadius;

                // Вычисляем альфу формы для текущего пикселя
                float shapeAlphaCenter = GetShapeAlpha(IN.uv, padX, padY, shapeType, corner);

                // Базовый цвет текстуры
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half alphaTex = texColor.a;

                // Итоговая альфа с учётом формы
                half finalAlpha = alphaTex * shapeAlphaCenter;

                // --- Outline с учётом формы (дилатация альфы формы) ---
                half outlineAlpha = finalAlpha;
                if (_OutlineWidth > 0.0)
                {
                    float2 offset = float2(_OutlineWidth, _OutlineWidth);
                    // Сэмплируем альфу текстуры для соседей и умножаем на альфу формы в этих точках
                    half alphaTL = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv - offset).a 
                                    * GetShapeAlpha(IN.uv - offset, padX, padY, shapeType, corner);
                    half alphaTR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2( offset.x, -offset.y)).a
                                    * GetShapeAlpha(IN.uv + float2( offset.x, -offset.y), padX, padY, shapeType, corner);
                    half alphaBL = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-offset.x,  offset.y)).a
                                    * GetShapeAlpha(IN.uv + float2(-offset.x,  offset.y), padX, padY, shapeType, corner);
                    half alphaBR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + offset).a
                                    * GetShapeAlpha(IN.uv + offset, padX, padY, shapeType, corner);

                    half maxAlpha = max(max(alphaTL, alphaTR), max(alphaBL, alphaBR));
                    outlineAlpha = max(maxAlpha, finalAlpha);
                }

                // Разделяем основной цвет и обводку
                half edge = outlineAlpha * (1.0 - finalAlpha);
                half3 baseRGB = texColor.rgb * IN.color.rgb;
                half3 outlineRGB = _OutlineColor.rgb;

                // --- Градиент ---
                #ifdef _GRADIENT_ENABLE
                    float gradFactor = (_GradientDirection < 0.5) ? IN.uv.y : IN.uv.x;
                    half3 gradColor = lerp(_GradientColorBottom.rgb, _GradientColorTop.rgb, gradFactor);
                    baseRGB = lerp(baseRGB, gradColor, _GradientBlend);
                #endif

                // --- Rim ---
                #ifdef _RIM_ENABLE
                    // Расстояние до края формы: 1 - shapeAlphaCenter (0 внутри, 1 снаружи)
                    float rimDist = 1.0 - shapeAlphaCenter;
                    float rimFactor = pow(saturate(rimDist / _RimWidth), _RimPower);
                    half3 rimColor = _RimColor.rgb * rimFactor * _RimColor.a;
                    baseRGB += rimColor;
                #endif

                // --- Highlight ---
                #ifdef _HIGHLIGHT_ENABLE
                    float angleRad = radians(_HighlightAngle);
                    float2 dir = float2(cos(angleRad), sin(angleRad));
                    float proj = dot(IN.uv - 0.5, dir);
                    float pos = _HighlightPosition;
                    float halfW = _HighlightWidth * 0.5;
                    float highlight = 1.0 - saturate(abs(proj - pos) / halfW);
                    highlight = pow(highlight, 2);
                    half3 highlightColor = _HighlightColor.rgb * highlight * _HighlightColor.a * _HighlightBlend;
                    baseRGB += highlightColor;
                #endif

                // Итоговый цвет
                half3 finalRGB = lerp(baseRGB, outlineRGB, edge);
                half finalA = max(finalAlpha * IN.color.a, edge * _OutlineColor.a);

                return half4(finalRGB, finalA);
            }
            ENDHLSL
        }
    }
    FallBack "UI/Default"
}