Shader "BoM/Eye" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
		_AlphaThreshold ("Alpha Threshold", Range (0, 1)) = 0.5
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader {
		Tags {"RenderType"="Opaque"} // "Queue"="Transparent"  "IgnoreProjector"="True"
		Lighting On
		LOD 200
		
		CGPROGRAM
			#pragma surface surf BlinnPhong
			
			fixed4 _Color;
			fixed4 _BackgroundColor;
			fixed _AlphaThreshold;
			sampler2D _MainTex;
			
			struct Input {
				float2 uv_MainTex;
			};
			
			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				
				if(c.a >= _AlphaThreshold) {
					o.Albedo = c.rgb * _Color.rgb;
				} else {
					o.Albedo = _BackgroundColor; // c.rgb * 
				}
				
				o.Alpha = 1;
			}
		ENDCG
	}
	
	Fallback "Transparent/VertexLit"
}