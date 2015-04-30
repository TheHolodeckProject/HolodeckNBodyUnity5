using UnityEngine;
using System.Collections;

public class CameraScreenEffects : MonoBehaviour {
	
	public GameObject CameraL;
	public GameObject CameraR;
	private CC_Blend CameraLLightEffect;
	private CC_Blend CameraRLightEffect;
	private Tonemapping CameraLTonemapperEffect;
	private Tonemapping CameraRTonemapperEffect;
	private float fadeDirection;
	private float amount = 1.0f;
	public float fadeTime = 0.7f;
	public bool isDebug = false;

	// Use this for initialization
	void Start () {
		CameraLLightEffect = CameraL.GetComponent<CC_Blend>();
		CameraRLightEffect = CameraR.GetComponent<CC_Blend>();
		FadeIn ();
	}

	// Update is called once per frame
	void Update () {

		amount -= fadeDirection * fadeTime * Time.deltaTime;
		amount = Mathf.Clamp01 (amount);
		CameraLLightEffect.amount = amount;
		CameraRLightEffect.amount = amount;

		if (isDebug) {
			if (Input.GetKeyDown ("space")) {
				FadeOut ();
			}

			if (Input.GetKeyUp ("space")) {
				FadeIn ();
			}

		}

	}

	void FadeIn () {
		fadeDirection = 1;
	}

	void FadeOut () {
		fadeDirection = -1;
	}
}
