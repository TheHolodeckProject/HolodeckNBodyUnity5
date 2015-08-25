using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Nebula Starfield")]
public class SgtNebulaStarfield : SgtStarfield
{
	public float Radius = 100.0f;
	
	[SgtSeedAttribute]
	public int Seed;
	
	public Texture SourceTex;
	
	[SgtRangeAttribute(0.0f, 1.0f)]
	public float Resolution = 0.1f;
	
	[SgtRangeAttribute(0.0f, 1.0f)]
	public float Threshold = 0.1f;
	
	public SgtNebulaSource HeightSource = SgtNebulaSource.None;
	
	public Vector3 Size = new Vector3(100.0f, 100.0f, 100.0f);
	
	public float HorizontalBrightness = 0.25f;
	
	public float HorizontalPower = 1.0f;
	
	public float StarRadiusMin = 0.0f;
	
	public float StarRadiusMax = 1.0f;
	
	[SgtRangeAttribute(0.0f, 1.0f)]
	public float StarPulseMax = 1.0f;
	
	public List<Sprite> StarSprites = new List<Sprite>();
	
	public override void ObserverPreCull(SgtObserver observer)
	{
		base.ObserverPreCull(observer);
		
		var dir    = (transform.position - observer.transform.position).normalized;
		var theta  = Mathf.Abs(Vector3.Dot(transform.up, dir));
		var bright = Mathf.Lerp(HorizontalBrightness, Brightness, Mathf.Pow(theta, HorizontalPower));
		var color  = SgtHelper.Brighten(Color, bright);
		
		for (var i = groups.Count - 1; i >= 0; i--)
		{
			var group = groups[i];
			
			if (group != null)
			{
				if (group.Material != null)
				{
					group.Material.SetColor("_Color", color);
				}
			}
		}
	}
	
	public static SgtNebulaStarfield CreateNebulaStarfield(Transform parent = null)
	{
		return CreateNebulaStarfield(parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}
	
	public static SgtNebulaStarfield CreateNebulaStarfield(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Nebula Starfield", parent, localPosition, localRotation, localScale);
		var starfield  = gameObject.AddComponent<SgtNebulaStarfield>();
		
		return starfield;
	}
	
	protected override void CalculateStars(out List<SgtStarfieldStar> stars, out bool pool)
	{
		var texture = SourceTex as Texture2D;
		
		stars = new List<SgtStarfieldStar>();
		pool  = true;
		
		if (texture != null && Resolution > 0.0f && Resolution <= 1.0f)
		{
#if UNITY_EDITOR
			SgtHelper.MakeTextureReadable(texture);
			SgtHelper.MakeTextureTruecolor(texture);
#endif
			var samplesX = Mathf.FloorToInt(SourceTex.width  * Resolution);
			var samplesY = Mathf.FloorToInt(SourceTex.height * Resolution);
			var stepX    = SgtHelper.Reciprocal(samplesX);
			var stepY    = SgtHelper.Reciprocal(samplesY);
			var halfSize = Size * 0.5f;
			var scale    = SgtHelper.Divide(SourceTex.width, samplesX);
			
			SgtHelper.BeginRandomSeed(Seed);
			{
				for (var y = 0; y < samplesY; y++)
				{
					for (var x = 0; x < samplesX; x++)
					{
						var fracX = x * stepX;
						var fracY = y * stepY;
						var pixel = texture.GetPixelBilinear(fracX, fracY);
						var gray  = pixel.grayscale;
						
						if (gray > Threshold)
						{
							var star      = SgtClassPool<SgtStarfieldStar>.Pop() ?? new SgtStarfieldStar(); stars.Add(star);
							var position  = -halfSize;
							
							position.x += Size.x * fracX;
							position.y += Size.y * GetHeight(pixel);
							position.z += Size.z * fracY;
							
							star.Sprite      = GetRandomStarSprite();
							star.Color       = pixel;
							star.Radius      = Random.Range(StarRadiusMin, StarRadiusMax) * scale;
							star.Angle       = Random.Range(0.0f, Mathf.PI * 2.0f);
							star.Position    = position;
							star.PulseRange  = Random.value * StarPulseMax;
							star.PulseSpeed  = Random.value;
							star.PulseOffset = Random.value;
						}
					}
				}
			}
			SgtHelper.EndRandomSeed();
		}
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		
		Gizmos.DrawWireCube(Vector3.zero, Size);
	}
#endif
	
	private Sprite GetRandomStarSprite()
	{
		if (StarSprites != null && StarSprites.Count > 0)
		{
			var index = Random.Range(0, StarSprites.Count);
			
			return StarSprites[index];
		}
		
		return null;
	}
	
	private float GetHeight(Color pixel)
	{
		switch (HeightSource)
		{
			case SgtNebulaSource.None: return 0.5f;
			case SgtNebulaSource.Red: return pixel.r;
			case SgtNebulaSource.Green: return pixel.g;
			case SgtNebulaSource.Blue: return pixel.b;
			case SgtNebulaSource.Alpha: return pixel.a;
			case SgtNebulaSource.AverageRgb: return (pixel.r + pixel.g + pixel.b) / 3.0f;
			case SgtNebulaSource.MinRgb: return Mathf.Min(pixel.r, Mathf.Min(pixel.g, pixel.b));
			case SgtNebulaSource.MaxRgb: return Mathf.Max(pixel.r, Mathf.Max(pixel.g, pixel.b));
		}
		
		return 0.0f;
	}
	
#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Nebula Starfield", false, 10)]
	private static void CreateNebulaStarfieldMenuItem()
	{
		var starfield = CreateNebulaStarfield(null);
		
		SgtHelper.SelectAndPing(starfield);
	}
#endif
}