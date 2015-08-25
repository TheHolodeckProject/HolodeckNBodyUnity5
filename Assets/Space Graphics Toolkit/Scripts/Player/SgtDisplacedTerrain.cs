using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Displaced Terrain")]
public class SgtDisplacedTerrain : SgtTerrain
{
	public Texture2D HeightTex;
	
	public float InnerRadius = 0.9f;
	
	public float OuterRadius = 1.1f;
	
	public override float GetHeight(Vector3 point)
	{
		if (HeightTex != null)
		{
			var uv = SgtHelper.CartesianToPolarUV(point);
			
			return Mathf.Lerp(InnerRadius, OuterRadius, GetHeightTexAlpha(uv));
		}
		
		return 1.0f;
	}
	
	public static SgtDisplacedTerrain CreateDisplacedTerrain(Transform parent = null)
	{
		return CreateDisplacedTerrain(parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}
	
	public static SgtDisplacedTerrain CreateDisplacedTerrain(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Displaced Terrain", parent, localPosition, localRotation, localScale);
		var terrain    = gameObject.AddComponent<SgtDisplacedTerrain>();
		
		return terrain;
	}
	
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		
		Gizmos.DrawWireSphere(Vector3.zero, InnerRadius);
		Gizmos.DrawWireSphere(Vector3.zero, OuterRadius);
	}
	
	private float GetHeightTexAlpha(Vector2 uv)
	{
		int px, py; float fx, fy;
		
		FracInt(uv.x * HeightTex.width , out px, out fx);
		FracInt(uv.y * HeightTex.height, out py, out fy);
		
		var w0 = GetPixel(px - 1, py - 1);
		var w1 = GetPixel(px    , py - 1);
		var w2 = GetPixel(px + 1, py - 1);
		var w3 = GetPixel(px + 2, py - 1);
		var x0 = GetPixel(px - 1, py    );
		var x1 = GetPixel(px    , py    );
		var x2 = GetPixel(px + 1, py    );
		var x3 = GetPixel(px + 2, py    );
		var y0 = GetPixel(px - 1, py + 1);
		var y1 = GetPixel(px    , py + 1);
		var y2 = GetPixel(px + 1, py + 1);
		var y3 = GetPixel(px + 2, py + 1);
		var z0 = GetPixel(px - 1, py + 2);
		var z1 = GetPixel(px    , py + 2);
		var z2 = GetPixel(px + 1, py + 2);
		var z3 = GetPixel(px + 2, py + 2);
		
		var a = Cubic(w0, w1, w2, w3, fx);
		var b = Cubic(x0, x1, x2, x3, fx);
		var c = Cubic(y0, y1, y2, y3, fx);
		var d = Cubic(z0, z1, z2, z3, fx);
		
		return Cubic(a, b, c, d, fy);
	}
	
	private float GetHeightTexAlphaCos(Vector2 uv)
	{
		int px, py; float fx, fy;
		
		FracInt(uv.x * HeightTex.width , out px, out fx);
		FracInt(uv.y * HeightTex.height, out py, out fy);
		
		var x1 = RepeatX(px);
		var x2 = RepeatX(px + 1);
		var y1 = ClampY(py);
		var y2 = ClampY(py + 1);
		var bl = HeightTex.GetPixel(x1, y1).a;
		var br = HeightTex.GetPixel(x2, y1).a;
		var tl = HeightTex.GetPixel(x1, y2).a;
		var tr = HeightTex.GetPixel(x2, y2).a;
		
		var b = Coserp(bl, br, fx);
		var t = Coserp(tl, tr, fx);
		
		return Coserp(b, t, fy);
	}
	
	private int RepeatX(int x)
	{
		if (x < 0) return HeightTex.width + x;
		if (x >= HeightTex.width) return x - HeightTex.width;
		
		return x;
	}
	
	private int ClampY(int y)
	{
		if (y < 0) return 0;
		if (y >= HeightTex.height) return HeightTex.height - 1;
		
		return y;
	}
	
	private float Coserp(float a, float b, float t)
	{
		t = (1.0f - Mathf.Cos(t * Mathf.PI)) * 0.5f;
		
		return a + (b - a) * t;
	}
	
	public static float Cubic(float a, float b, float c, float d, float t)
	{
		float aSq = t * t;
		d = (d - c) - (a - b);
		return d * (aSq * t) + ((a - b) - d) * aSq + (c - a) * t + b;
	}
	
	private void FracInt(float v, out int i, out float f)
	{
		i = (int)v;
		f = v - i;
	}
	
	private float GetPixel(int x, int y)
	{
		return HeightTex.GetPixel(RepeatX(x), ClampY(y)).a;
	}
	
#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Displaced Terrain", false, 10)]
	private static void CreateDisplacedTerrainMenuItem()
	{
		var terrain = CreateDisplacedTerrain(null);
		
		SgtHelper.SelectAndPing(terrain);
	}
#endif
}