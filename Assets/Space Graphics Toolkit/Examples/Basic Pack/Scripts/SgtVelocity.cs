using UnityEngine;

// This component allows you to view a rigidbody's current velocity, as well as set its initial velocity in the editor
[RequireComponent(typeof(Rigidbody))]
public class SgtVelocity : MonoBehaviour
{
	public Vector3 Velocity;
	
	[HideInInspector]
	[SerializeField]
	private Vector3 expectedVelocity;
	
	private new Rigidbody rigidbody;
	
	protected virtual void OnEnable()
	{
		UpdateVelocity(true);
	}
	
	protected virtual void Update()
	{
		UpdateVelocity();
	}
	
	protected virtual void FixedUpdate()
	{
		UpdateVelocity();
	}
	
	private void UpdateVelocity(bool forceSet = false)
	{
		if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();
		
		if (Velocity != expectedVelocity || forceSet == true)
		{
			rigidbody.velocity = expectedVelocity = Velocity;
		}
		else
		{
			Velocity = expectedVelocity = rigidbody.velocity;
		}
	}
}