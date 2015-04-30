using UnityEngine;
using System.Collections;

public class Puzzle_BigCubeHighlightMaterial : MonoBehaviour {

	public Material material_Cube_Highlight;
	public float pulsationDuration = 1.0f;
	public float pulsationMultPassive = 1.7f;
	private Color highlightColorRight = Color.white;
	private Color highlightColorWrong = Color.red;
	private Color highlightColorDefault = Color.gray;
	private float pulsationMultAction;
	private float pulsationMultWin;
	private float pulsationMultActionAndWinFadeTime = 1.0f;
	private float amplitude;
	private float highlightColor;
	public bool isDebug = false;

	// Use this for initialization
	void Start () {
		material_Cube_Highlight.color = highlightColorDefault;
	}
	
	// Update is called once per frame
	void Update () {
		//Passive Pulsation
		float phi = Time.time / pulsationDuration * 2 * Mathf.PI;
		amplitude = Mathf.Clamp(((Mathf.Cos(phi) * 0.5f + 0.5f) * pulsationMultPassive), 0.7f, pulsationMultPassive) + pulsationMultAction + pulsationMultWin;
		material_Cube_Highlight.SetFloat("_IllumPower", amplitude);

		//actions
		material_Cube_Highlight.color = Color.Lerp(material_Cube_Highlight.color, highlightColorDefault, Time.deltaTime * pulsationMultActionAndWinFadeTime);
		pulsationMultAction = Mathf.Lerp(pulsationMultAction, 0, Time.deltaTime * pulsationMultActionAndWinFadeTime);
		pulsationMultWin = Mathf.Lerp(pulsationMultWin, 0, Time.deltaTime * pulsationMultActionAndWinFadeTime);
		//Debug
		if (isDebug) {
			if (Input.GetKeyDown ("space")) {
				playerSetCubeRight ();
			}

			if (Input.GetKeyUp ("space")) {
				playerSetCubeWrong ();
			}
		}
	}

	void playerSetCubeRight () {
		material_Cube_Highlight.color = highlightColorRight;
		pulsationMultAction = 7.0f;
	}

	void playerSetCubeWrong () {
		material_Cube_Highlight.color = highlightColorWrong;
		pulsationMultAction = 7.0f;
	}
}
