Shader "Projector/Add" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)   	
		_ShadowTex ("Cookie", 2D) = "" { TexGen ObjectLinear }
	}
	Subshader {
		Pass {
			ZWrite off
			Fog { Color (0, 0, 0) }
			Color [_Color]
			ColorMask RGB
			Blend SrcAlpha One
			Offset -1, -1
			
			SetTexture [_ShadowTex] {
				combine texture * primary, ONE - texture
				Matrix [_Projector]
			}
		}
	}
}