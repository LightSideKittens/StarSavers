Shader "Custom/OffsetAnim"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_SpeedX ("Speed X", Range(-5,5)) = 1.0
        _SpeedY ("Speed Y", Range(-5,5)) = 1.0
        _Color ("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
        	Blend One One
            
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
            	fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };
            
			sampler2D _MainTex;
			float4 _MainTex_ST;
            float _SpeedX;
            float _SpeedY;
            fixed4 _Color;

			v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + float2(_Time.x * _SpeedX, _Time.x * _SpeedY);
				o.color = v.color * _Color;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				half4 color = tex2D(_MainTex, i.uv) * i.color;
				color.rgb *= color.a;
				return color;
			}
		ENDCG
		}
	}
}
