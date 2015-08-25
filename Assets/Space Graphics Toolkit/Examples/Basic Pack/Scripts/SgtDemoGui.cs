using UnityEngine;

// This component draws the FPS and adds some buttons the top left
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Demo GUI")]
public class SgtDemoGui : MonoBehaviour
{
	private float counter;
	
	private int frames;
	
	private float fps;
	
	protected virtual void Update()
	{
		counter += Time.deltaTime;
		frames  += 1;
		
		if (counter >= 1.0f)
		{
			fps = (float)frames / counter;
			
			counter = 0.0f;
			frames  = 0;
		}
	}
	
	protected virtual void OnGUI()
	{
		var r1 = new Rect(5, 50 + 55 * 0, 100, 50);
		var r2 = new Rect(5, 50 + 55 * 1, 100, 50);
		var r3 = new Rect(5, 50 + 55 * 2, 100, 50);
		
		if (GUI.Button(r1, "Reload") == true)
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if (GUI.Button(r2, "Prev") == true)
		{
			var index = Application.loadedLevel - 1;
			
			if (index < 0)
			{
				index = Application.levelCount - 1;
			}
			
			Application.LoadLevel(index);
		}
		
		if (GUI.Button(r3, "Next") == true)
		{
			var index = Application.loadedLevel + 1;
			
			if (index >= Application.levelCount)
			{
				index = 0;
			}
			
			Application.LoadLevel(index);
		}
		
		SgtTooltip.DrawText("FPS: " + fps.ToString("0"), TextAnchor.UpperLeft);
	}
}