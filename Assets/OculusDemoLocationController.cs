using UnityEngine;
using System.Collections;

public class OculusDemoLocationController : MonoBehaviour {

    public KeyCode forwardKey = KeyCode.UpArrow;
    public KeyCode backwardKey = KeyCode.DownArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode upKey = KeyCode.PageUp;
    public KeyCode downKey = KeyCode.PageDown;

    public float positionModifier = 0.01f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(forwardKey))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + positionModifier);
        if (Input.GetKey(backwardKey))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - positionModifier);
        if (Input.GetKey(rightKey))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + positionModifier, gameObject.transform.position.y, gameObject.transform.position.z);
        if (Input.GetKey(leftKey))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x - positionModifier, gameObject.transform.position.y, gameObject.transform.position.z);
        if (Input.GetKey(upKey))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + positionModifier, gameObject.transform.position.z);
        if (Input.GetKey(downKey))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - positionModifier, gameObject.transform.position.z);
	}
}
