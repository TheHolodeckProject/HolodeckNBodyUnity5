using UnityEngine;
using UnityEditor;

public abstract class SgtBelt_Editor<T> : SgtEditor<T>
	where T : SgtBelt
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
		
		DrawDefault("Age");
		
		DrawDefault("TimeScale");
		
		DrawDefault("AutoRegenerate");
		
		if (Any(t => t.AutoRegenerate == false))
		{
			if (Button("Regenerate") == true)
			{
				Each(t => t.Regenerate());
			}
		}
		
		Separator();
	}
}