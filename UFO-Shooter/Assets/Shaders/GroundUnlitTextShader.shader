Shader "Unlit/GroundUnlitTextShader"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (0,1,1,1)
        _EdgePower ("Edge Power", Range(0.1, 10)) = 2.0
        _GlowIntensity ("Glow Intensity", Range(0,5)) = 1.5

        _TopTex ("Top Texture", 2D) = "white" {}
        _TopGlowColor ("Top Glow Color", Color) = (0,1,0.5,1)
        _TopGlowIntensity ("Top Glow Intensity", Range(0,5)) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One   // адитивне світіння
        ZWrite Off
        Cull Back
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float3 viewDir   : TEXCOORD1;
                float2 uv        : TEXCOORD2;
            };

            fixed4 _EdgeColor;
            float _EdgePower;
            float _GlowIntensity;

            sampler2D _TopTex;
            fixed4 _TopGlowColor;
            float _TopGlowIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.normalDir = worldNormal;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Fresnel контур
                float fresnel = pow(1.0 - saturate(dot(normalize(i.viewDir), normalize(i.normalDir))), _EdgePower);
                fixed4 edgeGlow = fixed4(_EdgeColor.rgb * fresnel * _GlowIntensity, 1.0);

                // Перевіряємо чи грань верхня (y ~ 1)
                float topMask = step(0.9, normalize(i.normalDir).y);

                // Текстура з прозорістю тільки зверху
                fixed4 tex = tex2D(_TopTex, i.uv) * _TopGlowColor * _TopGlowIntensity;
                fixed4 topGlow = tex * topMask;

                return edgeGlow + topGlow;
            }
            ENDCG
        }
    }
}