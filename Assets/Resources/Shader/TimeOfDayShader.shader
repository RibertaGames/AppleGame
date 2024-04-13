Shader "Custom/TimeOfDayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TimeOfDay ("Time of Day", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _TimeOfDay;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate color based on time of day
                float time = frac(_TimeOfDay);
                if (time < 0.25)
                {
                    // Morning: Blue and white gradient
                    col.rgb += lerp(fixed3(1, 1, 1), fixed3(0, 0, 1), time / 0.25);
                }
                else if (time < 0.5)
                {
                    // Daytime: Orange gradient
                    col.rgb += lerp(fixed3(1, 1, 1), fixed3(1, 0.5, 0), (time - 0.25) / 0.25);
                }
                else if (time < 0.75)
                {
                    // Evening: Reddish-orange gradient
                    col.rgb += lerp(fixed3(1, 1, 1), fixed3(1, 0.2, 0), (time - 0.5) / 0.25);
                }
                else
                {
                    // Night: Dark blue
                    col.rgb += lerp(fixed3(1, 1, 1), fixed3(0, 0, 0.2), (time - 0.75) / 0.25);
                }

                return col;
            }
            ENDCG
        }
    }
}