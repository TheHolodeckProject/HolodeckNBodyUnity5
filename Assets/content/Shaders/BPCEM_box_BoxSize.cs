using UnityEngine;
using System.Collections;

[ExecuteInEditMode] // Make mirror live-update even when not in play mode
public class BPCEM_box_BoxSize : MonoBehaviour {

//	Box start: pos of your object - size of your object / 2
//	box size: Dimensions of your object

	public Vector3 boxStart;
	public Vector3 boxSize;

	// Use this for initialization
	void Start() {
		boxStart = transform.position - (GetComponent<Renderer>().bounds.size * 0.5f);
		boxSize = GetComponent<Renderer>().bounds.size;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
