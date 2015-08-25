using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtDisplacedTerrain))]
public class SgtDisplacedTerrain_Editor : SgtTerrain_Editor<SgtDisplacedTerrain>
{
	protected override void OnInspector()
	{
		base.OnInspector();
		
		EditorGUI.BeginChangeCheck();
		{
			BeginError(Any(t => t.HeightTex == null));
			{
				DrawDefault("HeightTex");
			}
			EndError();
			
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
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkMeshAsDirty());
		}
		
		Each(t => SgtHelper.MakeTextureReadable(t.HeightTex));
	}
}