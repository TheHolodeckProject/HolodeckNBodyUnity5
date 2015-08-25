using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Custom Belt")]
public class SgtCustomBelt : SgtBelt
{
	public List<SgtBeltAsteroid> Asteroids = new List<SgtBeltAsteroid>();
	
	public static SgtCustomBelt CreateCustomBelt(Transform parent = null)
	{
		return CreateCustomBelt(parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}
	
	public static SgtCustomBelt CreateCustomBelt(Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Custom Belt", parent, localPosition, localRotation, localScale);
		var belt       = gameObject.AddComponent<SgtCustomBelt>();
		
		return belt;
	}
	
	protected override void CalculateAsteroids(out List<SgtBeltAsteroid> asteroids, out bool pool)
	{
		asteroids = Asteroids;
		pool      = false;
	}
	
#if UNITY_EDITOR
	[UnityEditor.MenuItem(SgtHelper.GameObjectMenuPrefix + "Custom Belt", false, 10)]
	public static void CreateCustomBeltMenuItem()
	{
		var belt = CreateCustomBelt(null);
		
		SgtHelper.SelectAndPing(belt);
	}
#endif
}