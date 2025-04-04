Shader "Custom/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1.0
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _SilhouetteColor ("Silhouette Color", Color) = (0, 0, 0, 1)
        [Toggle] _UseAlphaChannel ("Use Alpha Channel", Float) = 1
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
            float4 _MainTex_TexelSize;
            float _OutlineWidth;
            fixed4 _OutlineColor;
            fixed4 _SilhouetteColor;
            float _UseAlphaChannel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            // 알파값 검사 함수
            bool hasAlpha(float2 uv) {
                fixed4 tex = tex2D(_MainTex, uv);
                return _UseAlphaChannel > 0.5 ? tex.a > 0.1 : length(tex.rgb) > 0.1;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 originalColor = tex2D(_MainTex, i.uv);
                
                // 텍스처 알파가 없으면 투명 픽셀
                if (_UseAlphaChannel > 0.5 && originalColor.a < 0.1)
                    return fixed4(0, 0, 0, 0);
                    
                // RGB 값이 모두 0에 가까우면 투명 픽셀 (검은색 제외)
                if (_UseAlphaChannel < 0.5 && length(originalColor.rgb) < 0.1)
                    return fixed4(0, 0, 0, 0);
                
                // 주변 픽셀 검사를 위한 텍셀 크기 계산
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineWidth;
                
                // 테두리 여부 확인
                bool isOutline = false;
                
                // 현재 픽셀이 불투명하고 주변에 투명 픽셀이 있으면 테두리로 간주
                if (hasAlpha(i.uv)) {
                    // 인접한 8방향 픽셀 검사
                    for (int x = -1; x <= 1; x++) {
                        for (int y = -1; y <= 1; y++) {
                            if (x == 0 && y == 0) continue; // 현재 픽셀 제외
                            
                            float2 offset = float2(x, y) * texelSize;
                            if (!hasAlpha(i.uv + offset)) {
                                isOutline = true;
                                break;
                            }
                        }
                        if (isOutline) break;
                    }
                }
                
                // 테두리면 테두리 색상, 아니면 실루엣 색상 반환
                fixed4 result = isOutline ? _OutlineColor : _SilhouetteColor;
                result.a = originalColor.a;
                
                return result * i.color;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}