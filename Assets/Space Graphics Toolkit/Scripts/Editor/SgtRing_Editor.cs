using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtRing))]
public class SgtRing_Editor : SgtEditor<SgtRing>
{
	protected override void OnInspector()
	{
		DrawDefault("Lights");
		
		DrawDefault("Shadows");
		
		Separator();
		
		DrawDefault("Color");
		
		BeginError(Any(t => t.Brightness < 0.0f));
		{
			DrawDefault("Brightness");
		}
		EndError();
		
		DrawDefault("RenderQueue");
		
		DrawDefault("RenderQueueOffset");
		
		Separator();
		
		BeginError(Any(t => t.MainTex == null));
		{
			DrawDefault("MainTex");
		}
		EndError();
		
		DrawDefault("LightingBias");
		
		DrawDefault("LightingSharpness");
		
		DrawDefault("Scattering");
		
		if (Any(t => t.Scattering == true))
		{
			BeginIndent();
			{
				DrawDefault("MieSharpness");
				
				DrawDefault("MieStrength");
			}
			EndIndent();
		}
		
		Separator();
		
		EditorGUI.BeginChangeCheck();
		{
			BeginError(Any(t => t.InnerRadius < 0.0f || t.InnerRadius >= t.OuterRadius));
			{
				DrawDefault("InnerRadius");
			}
			EndError();
			
			BeginError(Any(t => t.OuterRadius < 0.0f || t.InnerRadius >= t.OuterRadius));
			{
				DrawDefault("OuterRadius");
			}
			EndError();
			
			BeginError(Any(t => t.SegmentCount < 1));
			{
				DrawDefault("SegmentCount");
			}
			EndError();
			
			BeginError(Any(t => t.SegmentDetail < 3));
			{
				DrawDefault("SegmentDetail");
			}
			EndError();
			
			BeginError(Any(t => t.BoundsShift < 0.0f));
			{
				DrawDefault("BoundsShift");
			}
			EndError();
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkMeshAsDirty());
		}
	}
}