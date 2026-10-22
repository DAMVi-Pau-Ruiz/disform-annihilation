Shader "Custom/OrganicMetaballs3D"
{
    Properties
    {
        _BlobSize ("Blob Size", Range(0.05, 0.4)) = 0.22
        _Threshold ("Fusion Threshold", Range(0.1, 4)) = 1.25
        _Speed ("Animation Speed", Range(0, 3)) = 0.8
        _Warp ("Organic Deformation", Range(0, 0.3)) = 0.06

        _EdgeWidth ("Edge Width", Range(0.005, 0.2)) = 0.04
        _Glow ("Glow Intensity", Range(0, 8)) = 3
        _HitFlash ("Hit Flash", Range(0, 1)) = 0

        _ColorA ("Cyan", Color) = (0.05, 0.9, 1, 1)
        _ColorB ("Purple", Color) = (0.55, 0.08, 1, 1)
        _ColorC ("Green", Color) = (0.2, 1, 0.35, 1)

        _Steps ("Raymarch Steps", Range(24, 128)) = 80
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "OrganicMetaballs"

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _BlobSize;
                float _Threshold;
                float _Speed;
                float _Warp;
                float _EdgeWidth;
                float _Glow;
                float _HitFlash;
                float4 _ColorA;
                float4 _ColorB;
                float4 _ColorC;
                float _Steps;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.positionOS = input.positionOS.xyz;

                return output;
            }

            float Hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;

                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            float BlobField(float3 p)
            {
                float t = _Time.y * _Speed;
                float field = 0.0;

                // Eight animated metaballs.
                // Their fields combine to create organic shapes.

                [unroll]
                for (int i = 0; i < 8; i++)
                {
                    float fi = (float)i;

                    float phase = fi * 2.3999632;

                    float3 center;

                    center.x = sin(t * 0.71 + phase) * 0.23;
                    center.y = cos(t * 0.83 + phase * 1.7) * 0.23;
                    center.z = sin(t * 0.59 + phase * 0.8) * 0.23;

                    center += 0.12 * float3(
                        sin(phase * 2.1),
                        cos(phase * 1.3),
                        sin(phase * 0.7)
                    );

                    float radius = _BlobSize *
                        (0.8 + 0.25 * sin(t + fi * 1.7));

                    float3 delta = p - center;

                    float d2 = dot(delta, delta);

                    field += (radius * radius) /
                        max(d2, 0.0001);
                }

                // Gentle organic surface deformation.

                float noise =
                    sin(p.x * 17.0 + t) *
                    sin(p.y * 15.0 - t * 0.7) *
                    sin(p.z * 19.0 + t * 0.8);

                field += noise * _Warp;

                return field;
            }

            float3 EstimateNormal(float3 p)
            {
                const float e = 0.002;

                float3 n;

                n.x = BlobField(p + float3(e, 0, 0))
                    - BlobField(p - float3(e, 0, 0));

                n.y = BlobField(p + float3(0, e, 0))
                    - BlobField(p - float3(0, e, 0));

                n.z = BlobField(p + float3(0, 0, e))
                    - BlobField(p - float3(0, 0, e));

                return normalize(-n);
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float3 rayOrigin =
                    TransformWorldToObject(_WorldSpaceCameraPos);

                float3 rayDirection =
                    normalize(input.positionOS - rayOrigin);

                // Intersect the ray with the unit cube.

                float3 invDir = 1.0 /
                    (rayDirection + sign(rayDirection) * 0.00001);

                float3 t0 = (-0.5 - rayOrigin) * invDir;
                float3 t1 = ( 0.5 - rayOrigin) * invDir;

                float3 tMin = min(t0, t1);
                float3 tMax = max(t0, t1);

                float tEnter = max(
                    max(tMin.x, tMin.y),
                    tMin.z
                );

                float tExit = min(
                    min(tMax.x, tMax.y),
                    tMax.z
                );

                if (tExit < max(tEnter, 0.0))
                    discard;

                float t = max(tEnter, 0.0);

                float hit = 0.0;
                float3 hitPosition = 0;

                int steps = (int)_Steps;

                float stepSize = (tExit - t) / max(steps, 1);

                // Raymarch through the volume.

                [loop]
                for (int i = 0; i < 128; i++)
                {
                    if (i >= steps)
                        break;

                    float3 p = rayOrigin + rayDirection * t;

                    float field = BlobField(p);

                    if (field >= _Threshold)
                    {
                        hit = 1.0;
                        hitPosition = p;
                        break;
                    }

                    t += stepSize;
                }

                if (hit < 0.5)
                    discard;

                float field = BlobField(hitPosition);

                float3 normal = EstimateNormal(hitPosition);

                // Color variation.

                float variation = sin(
                    hitPosition.x * 8.0 +
                    hitPosition.y * 7.0 +
                    hitPosition.z * 6.0 +
                    _Time.y * _Speed
                );

                float3 color = lerp(
                    _ColorA.rgb,
                    _ColorB.rgb,
                    saturate(variation * 0.5 + 0.5)
                );

                color = lerp(
                    color,
                    _ColorC.rgb,
                    saturate(
                        sin(hitPosition.y * 9.0 + _Time.y) * 0.5
                        + 0.5
                    ) * 0.35
                );

                // Approximate the surface edge from the field.

                float edge = saturate(
                    abs(field - _Threshold) /
                    max(_EdgeWidth, 0.001)
                );

                edge = 1.0 - edge;
                edge = pow(saturate(edge), 2.0);

                // Add a fresnel-like rim.

                float fresnel = pow(
                    1.0 - saturate(
                        dot(normal, normalize(-rayDirection))
                    ),
                    2.5
                );

                float glow = edge * _Glow + fresnel * 1.5;

                color *= 0.35 + fresnel * 0.65;
                color += lerp(_ColorA.rgb, _ColorB.rgb, fresnel)
                    * glow;

                color = lerp(color, float3(1.0, 1.0, 1.0), saturate(_HitFlash));

                return float4(color, 1.0);
            }

            ENDHLSL
        }
    }
}