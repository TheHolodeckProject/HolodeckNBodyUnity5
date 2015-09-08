using UnityEngine;
using System.Collections;

public class PulseColor : MonoBehaviour {
    public Color startColor;
    public Color endColor;
    public float pulseIntensity;
    public float pulseFrequencyFixedUpdateCycles = 60;

    private float t;
	// Use this for initialization
	void Start () {
        t = 0;
	}
	
	// Update is called once per frame
	void Update () {
        float x = Mathf.Sin(t / pulseFrequencyFixedUpdateCycles) * pulseIntensity;
        gameObject.GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, x);
        t++;
	}
}
