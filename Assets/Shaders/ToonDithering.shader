Shader "Unlit/ToonDithering"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 0, 0, 1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _Detail ("Details", Range(0, 1)) = 2
        _DitherPattern ("Dithering Pattern", 2D) = "white" {}
        _DitherSize ("Dithering Size", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                half3 normal: NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPosition : TEXCOORD1;
                half3 worldNormal : NORMAL;
            };

            half4 _MainColor;
            half4 _ShadowColor;
            float _Detail;
            sampler2D _DitherPattern;
            float4 _DitherPattern_TexelSize;
            float _DitherSize;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                float2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                float ditherValue = tex2D(_DitherPattern, ditherCoordinate * _DitherSize).r;

                float normalToLightning = max(0.0, dot(normalize(i.worldNormal), normalize(_WorldSpaceLightPos0.xyz)));

                float4 ditheredShadow = step(ditherValue, _ShadowColor);

                float4 col = lerp(ditheredShadow, _MainColor, floor(normalToLightning / _Detail));
                return col;
            }
            ENDCG
        }
        
        //Pass for Casting Shadows 
        Pass {
            Name "CastShadow"
            Tags { "LightMode" = "ShadowCaster" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            
            struct v2f 
            { 
                V2F_SHADOW_CASTER;
            };
            
            v2f vert( appdata_base v )
            {
                v2f o;
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            
            float4 frag( v2f i ) : COLOR
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
