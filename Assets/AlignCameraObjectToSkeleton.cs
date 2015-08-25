using UnityEngine;
using System.Collections;

public class AlignCameraObjectToSkeleton : MonoBehaviour {

    public GameObject cameraObject;
    public Vector3 fixedOffset;
    public GameObject bodyViewObject;
    private BodySourceView bodyViewScript;
    private GameObject bodyAttachmentPointObject;
    private GameObject body;
    private string bodyAttachmentPointName = "SpineShoulder";
    public Vector3 fixedRotation;

	// Use this for initialization
	void Start () {
        bodyViewScript = bodyViewObject.GetComponent<BodySourceView>();
        cameraObject.transform.Rotate(fixedRotation);
	}
	
	// Update is called once per frame
	void Update () {
        /*if (bodyViewScript.bodyObject != null)
        {
            body = bodyViewScript.bodyObject;
            bodyAttachmentPointObject = body.transform.FindChild(bodyAttachmentPointName).gameObject;
        }
        else
        {
            body = null;
            bodyAttachmentPointObject = null;
        }
        */
        if (body == null || bodyAttachmentPointObject == null) return;

        Debug.Log("Using Body: " + body.name);

        body.transform.position = new Vector3(cameraObject.transform.position.x, body.transform.position.y, cameraObject.transform.position.z);
	}
}
