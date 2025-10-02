Shader "Custom/EdgeGlow"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (0,0,1,1)
        _EdgePower ("Edge Power", Range(0.1, 10)) = 2.0
        _GlowIntensity ("Glow Intensity", Range(0,5)) = 1.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One One        //  адитивне світіння
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
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float3 viewDir   : TEXCOORD1;
            };

            fixed4 _EdgeColor;
            float _EdgePower;
            float _GlowIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.normalDir = worldNormal;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Fresnel для країв
                float fresnel = pow(1.0 - saturate(dot(normalize(i.viewDir), normalize(i.normalDir))), _EdgePower);

                // Яскравість підсвічування
                return fixed4(_EdgeColor.rgb * fresnel * _GlowIntensity, 1.0);
            }
            ENDCG
        }
    }
}