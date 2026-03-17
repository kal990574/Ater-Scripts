Shader "Custom/SonarScan"
{
    Properties
    {
        _ScanOrigin ("Scan Origin", Vector) = (0, 0, 0, 0)
        _ScanDirection ("Scan Direction", Vector) = (0, 0, 1, 0)
        _ScanRadius ("Scan Radius", Float) = 0
        _ScanMaxRadius ("Scan Max Radius", Float) = 15
        _ScanAngle ("Scan Angle", Float) = 40
        _RingWidth ("Ring Width", Float) = 2
        _ScanColor ("Scan Color", Color) = (0.4, 0.7, 1.0, 1.0)
        _EdgeThreshold ("Edge Threshold", Float) = 0.1
        _ScanLineFrequency ("Scan Line Frequency", Float) = 50
        _TrailIntensity ("Trail Intensity", Float) = 0.3
        _TrailFadeRadius ("Trail Fade Radius", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "SonarScanPass"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float4 _BlitTexture_TexelSize;
            float3 _ScanOrigin;
            float3 _ScanDirection;
            float _ScanRadius;
            float _ScanMaxRadius;
            float _ScanAngle;
            float _RingWidth;
            float4 _ScanColor;
            float _EdgeThreshold;
            float _ScanLineFrequency;
            float _TrailIntensity;
            float _TrailFadeRadius;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Full-screen 삼각형 생성
            Varyings Vert(Attributes input)
            {
                Varyings output;

                float2 uv = float2(
                    (input.vertexID << 1) & 2,
                    input.vertexID & 2
                );

                output.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);

                #if UNITY_UV_STARTS_AT_TOP
                    output.uv = float2(uv.x, 1.0 - uv.y);
                #else
                    output.uv = uv;
                #endif

                return output;
            }

            // Depth Buffer에서 월드 좌표 복원
            float3 GetWorldPosition(float2 uv)
            {
                #if UNITY_REVERSED_Z
                    float depth = SampleSceneDepth(uv);
                #else
                    float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
                #endif

                return ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);
            }

            // Sobel 에지 검출: 주변 8픽셀 깊이 차이로 윤곽선 추출
            float SobelDepthEdge(float2 uv)
            {
                float2 texelSize = _BlitTexture_TexelSize.xy;

                float d00 = SampleSceneDepth(uv + float2(-texelSize.x, -texelSize.y));
                float d10 = SampleSceneDepth(uv + float2(0, -texelSize.y));
                float d20 = SampleSceneDepth(uv + float2(texelSize.x, -texelSize.y));
                float d01 = SampleSceneDepth(uv + float2(-texelSize.x, 0));
                float d21 = SampleSceneDepth(uv + float2(texelSize.x, 0));
                float d02 = SampleSceneDepth(uv + float2(-texelSize.x, texelSize.y));
                float d12 = SampleSceneDepth(uv + float2(0, texelSize.y));
                float d22 = SampleSceneDepth(uv + float2(texelSize.x, texelSize.y));

                float gx = -d00 - 2.0 * d01 - d02 + d20 + 2.0 * d21 + d22;
                float gy = -d00 - 2.0 * d10 - d20 + d02 + 2.0 * d12 + d22;

                float edge = sqrt(gx * gx + gy * gy);
                return saturate(edge / _EdgeThreshold);
            }

            // 원뿔 마스크: 스캔 방향 기준 각도 내 픽셀만 통과
            float ConeMask(float3 worldPos)
            {
                float3 toPixel = normalize(worldPos - _ScanOrigin);
                float cosAngle = dot(toPixel, normalize(_ScanDirection));
                float cosHalfAngle = cos(radians(_ScanAngle * 0.5));
                float cosInner = cos(radians(_ScanAngle * 0.4));
                return smoothstep(cosHalfAngle, cosInner, cosAngle);
            }

            // 파동 링 마스크: 현재 반경 근처 픽셀 강조
            float RingMask(float dist)
            {
                return 1.0 - saturate(abs(dist - _ScanRadius) / _RingWidth);
            }

            // 잔상 마스크: _TrailFadeRadius ~ _ScanRadius 구간에만 표시
            float TrailMask(float dist)
            {
                float trailRange = _ScanRadius - _TrailFadeRadius;
                if (trailRange <= 0.001) return 0;

                // _TrailFadeRadius 이하는 소멸, _ScanRadius 이상은 미도달
                float inTrail = step(_TrailFadeRadius, dist) * step(dist, _ScanRadius);

                // 꼬리 안쪽(소멸 경계)에서 바깥(링)으로 갈수록 강해짐.
                float gradient = saturate((dist - _TrailFadeRadius) / trailRange);

                return inTrail * gradient * _TrailIntensity;
            }

            // 스캔 라인 패턴: 월드 Y 기반 수평 줄무늬
            float ScanLinePattern(float3 worldPos)
            {
                float scanLine = frac(worldPos.y * _ScanLineFrequency);
                return lerp(0.3, 1.0, step(0.5, scanLine));
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                half4 sceneColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv);

                // 스캔 비활성 시 원본 반환
                if (_ScanRadius <= 0)
                    return sceneColor;

                float3 worldPos = GetWorldPosition(uv);

                // 스카이박스/원거리 픽셀 제외
                #if UNITY_REVERSED_Z
                    float rawDepth = SampleSceneDepth(uv);
                    if (rawDepth < 0.0001)
                        return sceneColor;
                #else
                    float rawDepth = SampleSceneDepth(uv);
                    if (rawDepth > 0.9999)
                        return sceneColor;
                #endif

                float dist = distance(worldPos, _ScanOrigin);

                // 최대 반경 밖 제외
                if (dist > _ScanMaxRadius)
                    return sceneColor;

                float cone = ConeMask(worldPos);
                float ring = RingMask(dist);
                float trail = TrailMask(dist);
                float edge = SobelDepthEdge(uv);
                float scanLine = ScanLinePattern(worldPos);

                // 링 + 잔상 합성
                float scanMask = saturate(ring + trail) * cone;

                // 에지 + 스캔라인 합성
                float finalEffect = scanMask * edge * scanLine;

                half4 result = sceneColor + _ScanColor * finalEffect;
                return result;
            }
            ENDHLSL
        }
    }
}