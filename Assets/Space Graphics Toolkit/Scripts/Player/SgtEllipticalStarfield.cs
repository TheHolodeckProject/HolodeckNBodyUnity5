using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Elliptical Starfield")]
public class SgtEllipticalStarfield : SgtStarfield
{
	public float Radius = 1.0f;
	
	[SgtSeedAttribute]
	public int Seed;
	
	[SgtRangeAttribute(0.0f, 1.0f)]
	public float Symmetry = 1.0f;
	
	[SgtRangeAttribute(0.0f, 1.0f)]
	public float Offset = 0.0f;
	
	public bool Inverse;
	
	public int StarCount = 1000;
	
	public float StarRadiusMin = 0.0f;
	
	public float StarRadiusMax = 0.05f;
	
	[SgtRangeAttribute(0.0f, 1.0f)]
	public float StarPulseMax = 1.0f;
	
	public List<Sprite> StarSprites = new List<Sprite>();
	
	public static SgtEllipticalStarfield CreateEllipticalStarfield(Transform parent = null)
	{
		return CreateEllipticalStarfield(parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}
	
	public static SgtEllipticalStarfield CreateEllipticalStarfield(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Elliptical Starfield", parent, localPosition, localRotation, localScale);
		var starfield  = gameObject.AddComponent<SgtEllipticalStarfield>();
		
		return starfield;
	}
	
	protected override void CalculateStars(out List<SgtStarfieldStar> stars, out bool pool)
	{
		stars = new List<SgtStarfieldStar>();
		pool  = true;
		
		SgtHelper.BeginRandomSeed(Seed);
		{
			for (var i = 0; i < StarCount; i++)
			{
				var star      = SgtClassPool<SgtStarfieldStar>.Pop() ?? new SgtStarfieldStar(); stars.Add(star);
				var position  = Random.insideUnitSphere;
				var magnitude = Offset;
				
				if (Inverse == true)
				{
					magnitude += (1.0f - position.magnitude) * (1.0f - Offset);
				}
				else
				{
					magnitude += position.magnitude * (1.0f - Offset);
				}
				
				position.y *= Symmetry;
				
				star.Sprite      = GetRandomStarSprite();
				star.Color       = Color.white;
				star.Radius      = Random.Range(StarRadiusMin, StarRadiusMax);
				star.Angle       = Random.Range(0.0f, Mathf.PI * 2.0f);
				star.Position    = position.normalized * magnitude * Radius;
				star.PulseRange  = Random.value * StarPulseMax;
				star.PulseSpeed  = Random.value;
				star.PulseOffset = Random.value;
			}
		}
		SgtHelper.EndRandomSeed();
	}
	
#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		
		Gizmos.DrawWireSphere(Vector3.zero, Radius);
		
		Gizmos.DrawWireSphere(Vector3.zero, Radius * Offset);
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
	
#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Elliptical Starfield", false, 10)]
	private static void CreateEllipticalStarfieldMenuItem()
	{
		var starfield = CreateEllipticalStarfield(null);
		
		SgtHelper.SelectAndPing(starfield);
	}
#endif
}