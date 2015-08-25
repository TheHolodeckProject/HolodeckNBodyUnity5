using UnityEngine;
using System.Collections.Generic;

// This component allows gravity receivers to get attracted to it
[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Gravity Source")]
public class SgtGravitySource : MonoBehaviour
{
	public static List<SgtGravitySource> AllGravitySources = new List<SgtGravitySource>();
	
	public float Mass = 100.0f;
	
	private new Rigidbody rigidbody;
	
	protected virtual void OnEnable()
	{
		AllGravitySources.Add(this);
	}
	
	protected virtual void OnDisable()
	{
		AllGravitySources.Remove(this);
	}
	
	protected virtual void Update()
	{
		if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();
		
		if (rigidbody != null)
		{
			Mass = rigidbody.mass;
		}
	}
}