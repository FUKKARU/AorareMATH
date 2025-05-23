Shader "Custom/DistortShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _ColorA("Main Color A", Color) = (1,1,1,1)
        _ColorB("Main Color B", Color) = (1,1,1,1)
        _TintA("Edge Color A", Color) = (1,1,1,1)
        _TintB("Edge Color B", Color) = (1,1,1,1)
        _ScrollX("Scroll Noise X", Range(-10,10)) = 1
        _ScrollY("Scroll Noise Y", Range(-10,10)) = 1
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _DistortTex("UVDistort Texture", 2D) = "white" {}
        _Offset("Offset Gradient A/B", Range(-2,2)) = 1
        _Hard("Hard Cutoff", Range(1,40)) = 30
        _FillAmount("Fill Amount", Range(0,1)) = 0
        _Edge("Edge", Range(0,2)) = 1
        _Distort("Distort", Range(0,1)) = 0.2
        [MaterialToggle] SHAPE("Use Mask Texture", Float) = 0
        [MaterialToggle] SHAPEX("Multiply Noise", Float) = 0
        _ShapeTex("Mask", 2D) = "white" {}
        [Toggle] _FlipX("Flip X", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ SHAPE_ON
            #pragma multi_compile _ SHAPEX_ON
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD4;
                UNITY_FOG_COORDS(3)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _NoiseTex, _ShapeTex, _DistortTex;
            float4 _MainTex_ST, _DistortTex_ST, _ShapeTex_ST, _NoiseTex_ST;
            float4 _ColorA, _ColorB, _TintA, _TintB;
            float _Offset, _ScrollX, _ScrollY;
            float _FillAmount, _Edge, _Distort, _Hard, _FlipX;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _ShapeTex);
                o.uv2.x = lerp(o.uv2.x, 1 - o.uv2.x, _FlipX);
                o.uv3 = TRANSFORM_TEX(v.uv, _DistortTex);
                o.uv3.x = lerp(o.uv3.x, 1 - o.uv3.x, _FlipX);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 mainTexColor = tex2D(_MainTex, i.uv);
                float4 gradientMain = lerp(_ColorB, _ColorA, (i.uv3.x + _Offset)) * mainTexColor;
                float4 gradientTint = lerp(_TintB, _TintA, (i.uv3.x + _Offset));
                float4 gradientBlend = lerp(float4(2,2,2,2), float4(0,0,0,0), (i.uv3.x + (1 - _FillAmount) * 2));

                fixed4 uvdistort = tex2D(_DistortTex, i.uv3) * _Distort;
                fixed4 noise = tex2D(_NoiseTex, fixed2((i.uv.x + _Time.x * _ScrollX) + uvdistort.g, (i.uv.y + _Time.y * _ScrollY) + uvdistort.r));
                fixed4 shapetex = tex2D(_ShapeTex, i.uv2);

                #ifdef SHAPE_ON
                noise = 1 - (noise * (1 - _FillAmount) * 2 + (1 - (shapetex * _Hard)));
                #else
                noise += gradientBlend;
                #endif
                #ifdef SHAPEX_ON
                noise += gradientBlend;
                #endif

                float4 flame = saturate(noise.a * _Hard);
                float4 flamecolored = flame * gradientMain;
                float4 flamerim = saturate((noise.a + _Edge) * _Hard) - flame;
                float4 flamecolored2 = flamerim * gradientTint;
                float4 finalcolor = flamecolored + flamecolored2;
                return finalcolor;
            }
            ENDCG
        }
    }
}
