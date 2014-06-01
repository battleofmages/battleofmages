Shader "BoM/Hair" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
}

SubShader {
	Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}
	
	Cull Front
	ZWrite On
	ZTest Less
	Blend SrcAlpha OneMinusSrcAlpha
	//BlinnPhong alpha
	CGPROGRAM
		#pragma surface surf BlinnPhong alpha
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		half _Shininess;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};
		
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
	ENDCG
	
	Tags
    {
        "Queue" = "Transparent+1"
        "IgnoreProjector" = "True"
        "RenderType" = "Transparent"
    }
	
    Cull Back
    ZWrite On
    ZTest Less
    Blend SrcAlpha OneMinusSrcAlpha
	
	CGPROGRAM
		#pragma surface surf BlinnPhong alpha
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		half _Shininess;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};
		
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
	ENDCG
}

FallBack "Transparent/VertexLit"
}