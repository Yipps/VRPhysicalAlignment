Shader "Hidden/ALINE/Outline" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,0.5)
		_FadeColor ("Fade Color", Color) = (1,1,1,0.3)
		_PixelWidth ("Width (px)", Float) = 4
		_LengthPadding ("Length Padding (px)", Float) = 0
		_LineJoinThreshold ("Line Join Threshold", Range(-1,1)) = -0.6
	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Offset -3, -50
		Tags { "IgnoreProjector"="True" "RenderType"="Overlay" }
		// With line joins some triangles can actually end up backwards, so disable culling
		Cull Off
		
		// Render behind objects
		Pass {
			ZTest Greater
			
			HLSLPROGRAM
			#pragma require integers compute
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature UNITY_HDRP
			#include "aline_common.cginc"

			float4 _Color;
			float4 _FadeColor;
			float _PixelWidth;
			float _LengthPadding;
			float _LineJoinThreshold;
			StructuredBuffer<LineData> lineInstances;

			static const float FalloffTextureScreenPixels = 2;
			
			line_v2f vert (InstancedInput v, out float4 outpos : SV_POSITION) {
				uint vertexInLine = v.vertexID % 6;
				uint lineID = v.vertexID / 6;
				LineData instance = lineInstances[lineID];
				float2 uv = vertexToSide[vertexInLine];				
				LineData joinLine = lineInstances[lineID + decodeLineJoinOffset(instance.joinHintInstanceIndex, uv)];

				line_v2f o = line_vert2(v, instance, joinLine, uv, _PixelWidth, _LineJoinThreshold, _LengthPadding, outpos);
				o.col = instance.color * _Color * _FadeColor;
				o.col.rgb = ConvertSRGBToDestinationColorSpace(o.col.rgb);
				return o;
			}

			half4 frag (line_v2f i, float4 screenPos : VPOS) : COLOR {
				float a = calculateLineAlpha(i, i.lineWidth, FalloffTextureScreenPixels);
				return i.col * float4(1,1,1,a);
			}
			ENDHLSL
		}

		// First pass writes to the Z buffer
		// where the lines have a pretty high opacity
		Pass {
			ZTest LEqual
			ZWrite On
			ColorMask 0
			
			HLSLPROGRAM
			#pragma require integers compute
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature UNITY_HDRP
			#include "aline_common.cginc"
			
			float _PixelWidth;
			float _LengthPadding;
			float _LineJoinThreshold;
			StructuredBuffer<LineData> lineInstances;
			
			// Number of screen pixels that the _Falloff texture corresponds to
			static const float FalloffTextureScreenPixels = 2;
			
			line_v2f vert (InstancedInput v, out float4 outpos : SV_POSITION) {
				uint vertexInLine = v.vertexID % 6;
				uint lineID = v.vertexID / 6;
				LineData instance = lineInstances[lineID];
				float2 uv = vertexToSide[vertexInLine];				
				LineData joinLine = lineInstances[lineID + decodeLineJoinOffset(instance.joinHintInstanceIndex, uv)];

				line_v2f o = line_vert2(v, instance, joinLine, uv, _PixelWidth, _LineJoinThreshold, _LengthPadding, outpos);
				o.col = half4(1,1,1,1);
				return o;
			}

			half4 frag (line_v2f i, float4 screenPos : VPOS) : COLOR {
				float a = calculateLineAlpha(i, i.lineWidth, FalloffTextureScreenPixels);
				if (a < 0.7) discard;
				return float4(1,1,1,a);
			}
			ENDHLSL
		}
		
		// Render in front of objects
		Pass {
			ZTest LEqual
			
			HLSLPROGRAM
			#pragma require integers compute
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature UNITY_HDRP
			#include "aline_common.cginc"

			float4 _Color;
			float _PixelWidth;
			float _LengthPadding;
			float _LineJoinThreshold;
			StructuredBuffer<LineData> lineInstances;
			
			// Number of screen pixels that the _Falloff texture corresponds to
			static const float FalloffTextureScreenPixels = 2;
			
			line_v2f vert (InstancedInput v, out float4 outpos : SV_POSITION) {
				uint vertexInLine = v.vertexID % 6;
				uint lineID = v.vertexID / 6;
				LineData instance = lineInstances[lineID];
				float2 uv = vertexToSide[vertexInLine];				
				LineData joinLine = lineInstances[lineID + decodeLineJoinOffset(instance.joinHintInstanceIndex, uv)];

				line_v2f o = line_vert2(v, instance, joinLine, uv, _PixelWidth, _LineJoinThreshold, _LengthPadding, outpos);
				o.col = instance.color * _Color;
				o.col.rgb = ConvertSRGBToDestinationColorSpace(o.col.rgb);
				return o;
			}

			half4 frag (line_v2f i, float4 screenPos : VPOS) : COLOR {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float a = calculateLineAlpha(i, i.lineWidth, FalloffTextureScreenPixels);
				return i.col * float4(1,1,1,a);
			}
			ENDHLSL
		}
	}
	Fallback Off
}
