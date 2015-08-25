using UnityEngine;
using System.Collections;

public class VerticalBounce : MonoBehaviour {
    private Vector3 initialPosition;
    public float bounceIntensity = 0.05f;
    public float bounceFrequency = 100f;
    private int t;
	// Use this for initialization
	void Start () {
        initialPosition = transform.localPosition;
        t = 0;
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = new Vector3(initialPosition.x, bounceIntensity*Mathf.Sin(t/bounceFrequency)+initialPosition.y, initialPosition.z);
        t++;
	}

    public void SetNewPosition(Vector3 position)
    {
        initialPosition = position;
    }
}
