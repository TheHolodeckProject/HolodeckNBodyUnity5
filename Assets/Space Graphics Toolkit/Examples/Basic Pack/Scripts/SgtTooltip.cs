using UnityEngine;

// This component allows you to draw simple gui text on the screen
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Tooltip")]
public class SgtTooltip : MonoBehaviour
{
	[Multiline]
	public string Top;
	
	[Multiline]
	public string Bottom;
	
	private static GUIStyle whiteStyle;
	
	private static GUIStyle blackStyle;
	
	private static bool setupStyles;
	
	public static void DrawText(string text, TextAnchor anchor, int offset = 15)
	{
		if (string.IsNullOrEmpty(text) == false)
		{
			if (setupStyles == false)
			{
				setupStyles = true;
				
				whiteStyle = new GUIStyle();
				whiteStyle.fontSize  = 20;
				whiteStyle.fontStyle = FontStyle.Bold;
				whiteStyle.wordWrap  = true;
				whiteStyle.normal    = new GUIStyleState();
				whiteStyle.normal.textColor = Color.white;
				
				blackStyle = new GUIStyle();
				blackStyle.fontSize  = 20;
				blackStyle.fontStyle = FontStyle.Bold;
				blackStyle.wordWrap  = true;
				blackStyle.normal    = new GUIStyleState();
				blackStyle.normal.textColor = Color.black;
			}
			
			whiteStyle.alignment = anchor;
			blackStyle.alignment = anchor;
			
			var sw   = (float)Screen.width;
			var sh   = (float)Screen.height;
			var rect = new Rect(0, 0, sw, sh);
			
			if (anchor == TextAnchor.LowerLeft || anchor == TextAnchor.MiddleLeft || anchor == TextAnchor.UpperLeft)
			{
				rect.xMin += offset;
			}
			
			if (anchor == TextAnchor.LowerRight || anchor == TextAnchor.MiddleRight || anchor == TextAnchor.UpperRight)
			{
				rect.xMax -= offset;
			}
			
			if (anchor == TextAnchor.UpperLeft || anchor == TextAnchor.UpperCenter || anchor == TextAnchor.UpperRight)
			{
				rect.yMin += offset;
			}
			
			if (anchor == TextAnchor.LowerLeft || anchor == TextAnchor.LowerCenter || anchor == TextAnchor.LowerRight)
			{
				rect.yMax -= offset;
			}
			
			rect.x += 1;
			GUI.Label(rect, text, blackStyle);
			
			rect.x -= 2;
			GUI.Label(rect, text, blackStyle);
			
			rect.x += 1;
			rect.y += 1;
			GUI.Label(rect, text, blackStyle);
			
			rect.y -= 2;
			GUI.Label(rect, text, blackStyle);
			
			rect.y += 1;
			GUI.Label(rect, text, whiteStyle);
		}
	}
	
	public void OnGUI()
	{
		DrawText(Top, TextAnchor.UpperCenter);
		DrawText(Bottom, TextAnchor.LowerCenter);
	}
}