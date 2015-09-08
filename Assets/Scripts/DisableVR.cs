using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class DisableVR : MonoBehaviour {

	// Use this for initialization
	void Start () {
        VRSettings.enabled = false;
	}
}
