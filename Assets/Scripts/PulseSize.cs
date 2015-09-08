using UnityEngine;
using System.Collections;

public class PulseSize : MonoBehaviour {

    public float pulseIntensity;
    public float pulseFrequencyFixedUpdateCycles = 60;
    public float pulseMinSize;
    private Vector3 initialScale;
    private int t;

	// Use this for initialization
	void Start () {
        initialScale = gameObject.transform.localScale;
        t = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        gameObject.transform.localScale = initialScale * Mathf.Sin(t / pulseFrequencyFixedUpdateCycles) * pulseIntensity + (new Vector3(pulseMinSize,pulseMinSize,pulseMinSize));
        t++;
	}
}
