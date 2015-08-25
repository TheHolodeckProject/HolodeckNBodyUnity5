using UnityEngine;
using UnityEditor;

public class SgtTerrain_Editor<T> : SgtEditor<T>
	where T : SgtTerrain
{
	protected override void OnInspector()
	{
		EditorGUI.BeginChangeCheck();
		{
			BeginError(Any(t => t.Resolution <= 0));
			{
				DrawDefault("Resolution");
			}
			EndError();
			
			DrawDefault("SkirtThickness");
			
			BeginError(Any(t => t.MaxSplitsInEditMode < 0 || t.MaxSplitsInEditMode > t.SplitDistances.Length));
			{
				DrawDefault("MaxSplitsInEditMode");
			}
			EndError();
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkMeshAsDirty());
		}
		
		EditorGUI.BeginChangeCheck();
		{
			DrawDefault("MaxColliderDepth");
		}
		if (EditorGUI.EndChangeCheck() == true)
		{
			Each(t => t.MarkStateAsDirty());
		}
		
		DrawDefault("SplitDistances");
		
		DrawDefault("Material");
		
		DrawDefault("Corona");
		
		Separator();
	}
}