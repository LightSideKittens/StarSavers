Shader "Custom/OffsetAnim"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_SpeedX ("Speed X", Range(-5,5)) = 1.0
        _SpeedY ("Speed Y", Range(-5,5)) = 1.0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
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
			
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
            float _SpeedX;
            float _SpeedY;

			v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + float2(_Time.x * _SpeedX, _Time.x * _SpeedY);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv) * _Color;
				return color;
			}
		ENDCG
		}
	}
}
