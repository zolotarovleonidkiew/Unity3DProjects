Shader "Custom/surfaceShader"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Texture1("Texture 1", 2D) = "white" {}
        _Texture2("Texture 2", 2D) = "white" {}
        [Slider] _blendStrength("Blend Strength", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _Texture1;
        sampler2D _Texture2;

        struct Input
        {
            float2 uv_Texture1;
            float2 uv_Texture2;
        };

        half _Glossiness;
        half _Metallic;
        half _blendStrength;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 color1 = tex2D(_Texture1, IN.uv_Texture1);
            fixed4 color2 = tex2D(_Texture2, IN.uv_Texture2);

            // Albedo comes from a texture tinted by color
            color1.a = 1 - _blendStrength;
            color2.a = _blendStrength;

            fixed4 c = color1;

            o.Alpha = _blendStrength;

            o.Albedo = color1.rgb * (1 - _blendStrength) + color2.rgb * _blendStrength;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
