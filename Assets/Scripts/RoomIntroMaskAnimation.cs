using UnityEngine;
using System.Collections;

public class RoomIntroMaskAnimation : MonoBehaviour {

	public GameObject mask_00;
	public GameObject mask_01;
	public bool mask_00_FadeIn;
	public bool mask_01_FadeIn;
	public Color maskColor00;
	public float fadeDuration = 1f;

	// Use this for initialization
	void Start () {
		StartCoroutine (IntroAnimationStart());
	}
	
	// Update is called once per frame
	void Update () {
	
		if (mask_00_FadeIn) {
			mask_00.GetComponent<Renderer>().material.color = Color.Lerp(mask_00.GetComponent<Renderer>().material.color, maskColor00, Time.deltaTime * fadeDuration);
		}
		if (mask_01_FadeIn) {
			mask_01.GetComponent<Renderer>().material.color = Color.Lerp(mask_01.GetComponent<Renderer>().material.color, maskColor00, Time.deltaTime * fadeDuration);
		}
	}

	IEnumerator IntroAnimationStart () {
		//camera FadeOut
		yield return new WaitForSeconds (7.0f);
		mask_00_FadeIn = true;
		yield return new WaitForSeconds (3.0f);
		mask_01_FadeIn = true;
	}
}
