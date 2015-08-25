using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Observer")]
public class SgtObserver : MonoBehaviour
{
	public static List<SgtObserver> AllObservers = new List<SgtObserver>();
	
	public float RollAngle;
	
	public Quaternion RollQuataternion = Quaternion.identity;
	
	public Matrix4x4 RollMatrix = Matrix4x4.identity;
	
	public Vector3 DeltaPosition;
	
	public Vector3 Velocity;
	
	private Quaternion oldRotation = Quaternion.identity;
	
	private Vector3 oldPosition;
	
	private new Camera camera;
	
	protected virtual void OnPreCull()
	{
		if (camera == null) camera = GetComponent<Camera>();
		
		for (var i = SgtStarfield.AllStarfields.Count - 1; i >= 0; i--)
		{
			SgtStarfield.AllStarfields[i].ObserverPreCull(this);
		}
		
		for (var i = SgtBelt.AllBelts.Count - 1; i >= 0; i--)
		{
			SgtBelt.AllBelts[i].ObserverPreCull(this);
		}
		
		for (var i = SgtProminence.AllProminences.Count - 1; i >= 0; i--)
		{
			SgtProminence.AllProminences[i].ObserverPreCull(this);
		}
		
		for (var i = SgtCloudsphere.AllCloudspheres.Count - 1; i >= 0; i--)
		{
			SgtCloudsphere.AllCloudspheres[i].ObserverPreCull(this);
		}
	}
	
	protected virtual void OnPreRender()
	{
		if (camera == null) camera = GetComponent<Camera>();
		
		for (var i = SgtCorona.AllCoronas.Count - 1; i >= 0; i--)
		{
			SgtCorona.AllCoronas[i].SetCurrentCamera(camera);
		}
		
		for (var i = SgtAtmosphere.AllAtmospheres.Count - 1; i >= 0; i--)
		{
			SgtAtmosphere.AllAtmospheres[i].SetCurrentCamera(camera);
		}
		
		for (var i = SgtJovian.AllJovians.Count - 1; i >= 0; i--)
		{
			SgtJovian.AllJovians[i].SetCurrentCamera(camera);
		}
		
#if UNITY_EDITOR
		for (var i = SgtRing.AllRings.Count - 1; i >= 0; i--)
		{
			SgtRing.AllRings[i].UpdateState();
		}
#endif
	}
	
	protected virtual void OnPostRender()
	{
		if (camera == null) camera = GetComponent<Camera>();
		
		for (var i = SgtStarfield.AllStarfields.Count - 1; i >= 0; i--)
		{
			SgtStarfield.AllStarfields[i].ObserverPostRender(this);
		}
		
		for (var i = SgtProminence.AllProminences.Count - 1; i >= 0; i--)
		{
			SgtProminence.AllProminences[i].ObserverPostRender(this);
		}
		
		for (var i = SgtCloudsphere.AllCloudspheres.Count - 1; i >= 0; i--)
		{
			SgtCloudsphere.AllCloudspheres[i].ObserverPostRender(this);
		}
	}
	
	protected virtual void LateUpdate()
	{
		var newRotation   = transform.rotation;
		var newPosition   = transform.position;
		var deltaRotation = Quaternion.Inverse(oldRotation) * newRotation;
		var deltaPosition = oldPosition - newPosition;
		
		oldRotation = newRotation;
		oldPosition = newPosition;
		
		RollAngle        = (RollAngle - deltaRotation.eulerAngles.z) % 360.0f;
		RollQuataternion = Quaternion.Euler(new Vector3(0.0f, 0.0f, RollAngle));
		RollMatrix       = SgtHelper.Rotation(RollQuataternion);
		DeltaPosition    = deltaPosition;
		Velocity         = SgtHelper.Reciprocal(Time.deltaTime) * deltaPosition;
	}
	
	protected virtual void OnEnable()
	{
		AllObservers.Add(this);
		
		oldPosition = transform.position;
	}
	
	protected virtual void OnDisable()
	{
		AllObservers.Remove(this);
	}
}