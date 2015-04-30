using UnityEngine;
using System.Collections;

public class Puzzle_CubeMaterials : MonoBehaviour {

	public int state = 0; //0 - Passive, 1 - OnHover, 2 - Move, 3 - CubeIsStaticInBigCube
	public Material materialCubeStatePassive;
	public Material materialCubeStateOnHover;
	public Material materialCubeStateMove;
	public Material materialCubeHighlight;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Material[] materials = GetComponent<Renderer>().materials;
		if (state == 0) {
			materials [6] = materialCubeStatePassive;
		}
		if (state == 1) {
			materials [6] = materialCubeStateOnHover;
		}
		if (state == 2) {
			materials [6] = materialCubeStateMove;
		}
		if (state == 3) {
			materials [6] = materialCubeHighlight;
		}
		GetComponent<Renderer>().materials = materials;
	}
}
