
Shader "Custom/Gray"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GrayscaleAmount ("Grayscale Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _GrayscaleAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                // 그레이스케일 변환
                // Y = 0.299R + 0.587G + 0.114B (밝기 계산 공식)
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                
                // 원본과 그레이스케일 사이를 _GrayscaleAmount로 보간
                col.rgb = lerp(col.rgb, float3(gray, gray, gray), _GrayscaleAmount);
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}