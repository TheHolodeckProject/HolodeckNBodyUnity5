using UnityEngine;
using System.Collections;

public class ArrowRenderer : MonoBehaviour {

    public GameObject objectToPointTo;
    private LineRenderer renderer;
    public Color longDistanceColor;
    public Color shortDistanceColor;
    public float colorDivisor = 2f;
	// Use this for initialization
	void Start () {
        renderer = gameObject.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        renderer.SetPosition(0, transform.position);
        renderer.SetPosition(1, objectToPointTo.transform.position);
        Color currentColor = Color.Lerp(shortDistanceColor, longDistanceColor, Vector3.Distance(transform.position, objectToPointTo.transform.position) / colorDivisor);
        renderer.material.color = currentColor;
	}
}
