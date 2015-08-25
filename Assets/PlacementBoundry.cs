using UnityEngine;
using System.Collections;

public class PlacementBoundry : MonoBehaviour {
    public GameObject objectToRestrictPlacement;
	// Use this for initialization
	void Start () {
        objectToRestrictPlacement.transform.localScale = new Vector3(objectToRestrictPlacement.transform.localScale.x / transform.localScale.x, objectToRestrictPlacement.transform.localScale.y / transform.localScale.y, objectToRestrictPlacement.transform.localScale.z / transform.localScale.z);
	}
	
	// Update is called once per frame
	void Update () {
        if (objectToRestrictPlacement.transform.localPosition.x > 0.5f)
            objectToRestrictPlacement.transform.localPosition = new Vector3(0.5f, objectToRestrictPlacement.transform.localPosition.y, objectToRestrictPlacement.transform.localPosition.z);
        else if (objectToRestrictPlacement.transform.localPosition.x < -0.5f)
            objectToRestrictPlacement.transform.localPosition = new Vector3(-0.5f, objectToRestrictPlacement.transform.localPosition.y, objectToRestrictPlacement.transform.localPosition.z);

        if (objectToRestrictPlacement.transform.localPosition.y > 0.5f)
            objectToRestrictPlacement.transform.localPosition = new Vector3(objectToRestrictPlacement.transform.localPosition.x, 0.5f, objectToRestrictPlacement.transform.localPosition.z);
        else if (objectToRestrictPlacement.transform.localPosition.y < -0.5f)
            objectToRestrictPlacement.transform.localPosition = new Vector3(objectToRestrictPlacement.transform.localPosition.x, -0.5f, objectToRestrictPlacement.transform.localPosition.z);

        if (objectToRestrictPlacement.transform.localPosition.z > 0.5f)
            objectToRestrictPlacement.transform.localPosition = new Vector3(objectToRestrictPlacement.transform.localPosition.x, objectToRestrictPlacement.transform.localPosition.y, 0.5f);
        else if (objectToRestrictPlacement.transform.localPosition.z < -0.5f)
            objectToRestrictPlacement.transform.localPosition = new Vector3(objectToRestrictPlacement.transform.localPosition.x, objectToRestrictPlacement.transform.localPosition.y, -0.5f);
	}
}
