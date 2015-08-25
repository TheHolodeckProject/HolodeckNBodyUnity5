using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StimulusManager : MonoBehaviour {

	private int numberOfStimuli; //Static value representing number of stimuli for task
	public GameObject[] stimuli; //List of game objects (populated on Start())
	private GameObject[] activeStimuli; //List of active stimuli indicies
	private bool objectRegionSetting;
	//Reset positions form a line between points P0 and P1 with even distribution (starting at end points)
	public Vector3 resetPositionP0; //Position for obj0
	public Vector3 resetPositionP1; //Position for objN
	//Random Bounds determines the 3D box which bounds the possible random positions of each object
	//The point P0 determines one corner of the box while the point P1 determines the opposite corner
	public Vector3 standardRandomBoundsP0; //One corner of the standard bounding box
	public Vector3 standardRandomBoundsP1; //The opposite corner of the standard bounding box
	public Vector3 reducedRandomBoundsP0; //One corner of the reduced bounding box
	public Vector3 reducedRandomBoundsP1; //The opposite corner of the reduced bounding box
	private Vector3 randomBoundsP0; //One corner of the bounding box being used
	private Vector3 randomBoundsP1; //The opposite corner of the bounding box being used
	public Vector3 stimuliScale = new Vector3 (0.1f, 0.1f, 0.1f); //Override scale for all stimuli
	public Vector3 overlapPaddingFactor = new Vector3(0.5f,0.5f,0.5f);

	/// /////////////////////////////////////////
	/// INITIALIZATION CODE /////////////////////
	/// /////////////////////////////////////////
	
	void Start () {
		numberOfStimuli = PlayerPrefs.GetInt("Number of Stimuli");
		objectRegionSetting = PlayerPrefs.GetInt ("Object Region")!=0;
		if (objectRegionSetting) {
			randomBoundsP0 = standardRandomBoundsP0;
			randomBoundsP1 = standardRandomBoundsP1;
		} else {
			randomBoundsP0 = reducedRandomBoundsP0;
			randomBoundsP1 = reducedRandomBoundsP1;
		}

		//??? Why do you initialize numberOfStimuli but not expectedNumberOfTrials? Why do I get an error when I try to call subjectIdentifier the same way?
		//subID = PlayerPrefs.GetInt ("subjectIdentifier");
		expectedNumberOfTrials = PlayerPrefs.GetInt ("Number of Trials");

		decideRandomStimuli ();

		generateRandomPositions ();

		GameObject.Find ("Logger").GetComponent<Logger> ().BeginLogging ();
	}

	private void decideRandomStimuli()
	{
		if (activeStimuli != null)
						for (int i = 0; i < activeStimuli.Length; i++) {
								try {
										DestroyImmediate (activeStimuli [i]);
								} catch (UnityException) {
								}
						}
		activeStimuli = new GameObject[numberOfStimuli];
		//Create a knuth shuffle index list of random indicies within the range of possible stimuli
		int[] knuthShuffleList = new int[stimuli.Length];
		for (int i = 0; i < knuthShuffleList.Length; i++)
			knuthShuffleList [i] = i;
		for (int i = 0; i < stimuli.Length; i++) {
			int index = Random.Range(i,stimuli.Length-1);
			int tmp = knuthShuffleList[index];
			knuthShuffleList[index] = knuthShuffleList[i];
			knuthShuffleList[i] = tmp;
		}
		
		for (int i = 0; i < numberOfStimuli; i++) {
			activeStimuli[i] = (GameObject)Instantiate(stimuli[knuthShuffleList[i]]);
			activeStimuli[i].transform.localScale = stimuliScale;
			activeStimuli[i].transform.parent = this.gameObject.transform;
			activeStimuli[i].transform.GetChild (0).gameObject.AddComponent<SphereCollider>();
			activeStimuli[i].AddComponent<SimpleObjectLogger>();
			activeStimuli[i].AddComponent<StimuliBehavior>().touched = false;
		}
	}

	private int numRetries = 100;
	void generateRandomPositions(){
		//Position stimuli randomly according to settings (no overlaps)
		List<Rect> overlapCheckList = new List<Rect> ();
		int retries = numRetries;
		for (int i = 0; i < activeStimuli.Length; i++) {
			activeStimuli [i].transform.localPosition = new Vector3 (
				Random.Range (randomBoundsP0.x > randomBoundsP1.x ? randomBoundsP1.x : randomBoundsP0.x,
			              randomBoundsP0.x < randomBoundsP1.x ? randomBoundsP1.x : randomBoundsP0.x),
				Random.Range (randomBoundsP0.y > randomBoundsP1.y ? randomBoundsP1.y : randomBoundsP0.y,
			              randomBoundsP0.y < randomBoundsP1.y ? randomBoundsP1.y : randomBoundsP0.y),
				Random.Range (randomBoundsP0.z > randomBoundsP1.z ? randomBoundsP1.z : randomBoundsP0.z,
			              randomBoundsP0.z < randomBoundsP1.z ? randomBoundsP1.z : randomBoundsP0.z));
			//Check for overlapping boxes and regenerate box location if overlap occurs
			Rect newBox = new Rect(activeStimuli[i].transform.localPosition.x,
			                       activeStimuli[i].transform.localPosition.z,
			                       activeStimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.x*activeStimuli[i].transform.localScale.x,
			                       activeStimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.z*activeStimuli[i].transform.localScale.z);
			if(boxesOverlapArray(newBox,overlapCheckList)){
				retries--;
				if(retries <= 0){
					//Force quit and overlap to prevent hang
					retries = numRetries;
					overlapCheckList.Add (newBox);
					Debug.Log ("Unable to find proper placement of object. Too many objects or incorrect mesh bounds?");
				}
				else i--;
			}
			else{
				retries = numRetries;
				overlapCheckList.Add (newBox);
			}
		}
	}

	bool boxesOverlapArray(Rect box, List<Rect> boxArray) {
		for (var i = 0; i < boxArray.Count; i++)
			if (boxesOverlap(box, boxArray[i])) return true;
		return false;
	}
	
	//Reference: http://gamemath.com/2011/09/detecting-whether-two-boxes-overlap/
	bool boxesOverlap(Rect box0, Rect box1) {
		if (box0.x + box0.width < box1.x) return false;
		if (box0.x > box1.x + box1.width) return false;
		if (box0.y + box0.height < box1.y) return false;
		if (box0.y > box1.y + box1.height) return false;
		return true;
	}

	/// /////////////////////////////////////////
	/// TEST PROCEDURE CODE /////////////////////
	/// /////////////////////////////////////////

	private int phase = 0;
	private float phaseWaitTime = 0.500f; //in s
	private float phaseWaitTimeStart;
	private bool phaseInit = true;
	private float timerStart;
	private float testTime = -1f; //If negative, test will be triggered only. If positive, there will be both timer and trigger
	private float studyTime = 2.000f;//in s
	private float restTime = 3.000f; //in s
	private int numberOfCompletedTrials = 0;
	private int expectedNumberOfTrials;
	// Update is called once per frame
	void Update () {
				if (phase == 0) {
						objectTouchedCondition ();

						int numObjectsTouched = 0;
			for (int i = 0; i < activeStimuli.Length; i++)
				if (activeStimuli [i].GetComponent <StimuliBehavior> ().touched)
										numObjectsTouched++;
						if(phaseInit){
							timerStart = Time.time;
							phaseInit = false;
						}
						//if (numObjectsTouched == activeStimIndicies.Length){
						if(Time.time-timerStart<studyTime){
							labelString = (studyTime - (Time.time-timerStart))+"";
						}
						else{
								phase = 1;
								phaseWaitTimeStart = Time.time;
								phaseInit = true;
						}
				}
				if (phase == 1) {
					if(Time.time - phaseWaitTimeStart>=phaseWaitTime){
						if(phaseInit){
							//Reset stimuli positions
					for(int i = 0; i < activeStimuli.Length;i++)
						activeStimuli[i].transform.localPosition = Vector3.Lerp (resetPositionP0,resetPositionP1,(((float)i)*(1f/((float)activeStimuli.Length))));
					for (int i = 0; i < activeStimuli.Length; i++)
						activeStimuli [i].GetComponent <StimuliBehavior> ().touched = false;
							timerStart = Time.time;
							phaseInit = false;
							//Mouse Grab Code (enable grabbing)
							ForceUngrab();
							allowGrab = true;
							//End Mouse Grab Code
						}
						if((testTime>0?Time.time-timerStart<testTime:false)||!phaseTransitionTrigger()){
							//Test is going
							//Counting down; labelString = (testTime - (Time.time-timerStart))+"";
							objectTouchedCondition();
							labelString = (Time.time-timerStart).ToString ("0.00000")+", press Space to move on.";
						}
						else{
							labelString = "Test Over";
							phase = 2;
							phaseWaitTimeStart = Time.time;
							phaseInit = true;
							//Mouse Grab Code (disable grabbing)
							ForceUngrab();
							allowGrab = false;
							//End Mouse Grab Code
						}
					}
				}
				if (phase == 2) {
					if(Time.time - phaseWaitTimeStart>=phaseWaitTime){
						if(phaseInit){
							timerStart = Time.time;
							phaseInit = false;
						}
						if(Time.time-timerStart<restTime){
							//Test is going
							labelString = "Resetting in " + (restTime - (Time.time-timerStart));
						}
						else{
							numberOfCompletedTrials++;
							if(numberOfCompletedTrials<expectedNumberOfTrials){
								labelString = "Click Objects";
								phase = 0;
								GameObject.Find ("Logger").GetComponent<Logger> ().Pause ();
								GameObject.Find ("Logger").GetComponent<Logger> ().FinishTrial (numberOfCompletedTrials);
								decideRandomStimuli ();
								generateRandomPositions ();
								GameObject.Find ("Logger").GetComponent<Logger> ().GenerateLoggableObjectsList();
								GameObject.Find ("Logger").GetComponent<Logger> ().Resume ();
								phaseWaitTimeStart = Time.time;
								phaseInit = true;
							}
							else{
								GameObject.Find ("Logger").GetComponent<Logger>().Finish ();
								//Trials all complete
								Application.LoadLevel (1);
							}
						}
					}
				}
			DragUpdate (); //Part of mouse drag code
		}

	bool phaseTransitionTrigger(){
		int numObjectsTouched = 0;
		for (int i = 0; i < activeStimuli.Length; i++)
			if (activeStimuli [i].GetComponent <StimuliBehavior> ().touched)
				numObjectsTouched++;
		return Input.GetKeyDown(KeyCode.Space) && activeStimuli.Length==numObjectsTouched;
	}

	/// /////////////////////////////////////////
	/// GUI OUTPUT CODE ////////////////////////
	/// /////////////////////////////////////////

	private string labelString = "Click Objects";
	void OnGUI () {
		// Make a background box
		GUI.Label(new Rect(10,10,100,90), labelString);
	}

	/// /////////////////////////////////////////
	/// TOUCH CODE //////////////////////////////
	/// /////////////////////////////////////////

	void objectTouchedCondition(){
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				for(int i = 0; i < activeStimuli.Length;i++){
					if(hit.collider.gameObject == activeStimuli[i].transform.GetChild (0).gameObject){
						activeStimuli[i].GetComponent<StimuliBehavior>().touched = true;
					}
				}
			}
		}
	}

	/// /////////////////////////////////////////
	/// CLICK AND DRAG CODE /////////////////////
	/// /////////////////////////////////////////

	Transform grabbed;
	bool rightClickGrab = false;
	float grabDistance = 100.0f;
	bool allowGrab = false;
	void DragUpdate () {
		UpdateHoldDrag();
	}
	
	// Drags when user holds down button
	void UpdateHoldDrag () {
		if (Input.GetMouseButton (0)) {
			if (grabbed)
				Drag ();
			else {
				rightClickGrab = false;
				Grab ();
			}
		}
		else if(Input.GetMouseButton (1)){
			if (grabbed)
				Drag ();
			else {
				rightClickGrab = true;
				Grab ();
			}
		}
		else
			grabbed = null;
	}

	void ForceUngrab()
	{
		grabbed = null;
	}

	void Grab() {
		if (allowGrab) {
			if (grabbed)
				grabbed = null;
			else {
				RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if (Physics.Raycast (ray, out hit)) {
					for (int i = 0; i < activeStimuli.Length; i++) {
						if (hit.collider.gameObject == activeStimuli [i].transform.GetChild (0).gameObject) {
								grabbed = hit.transform.parent;
							}
						}
					}
				}
		}
	}
	
	void Drag() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 position = transform.position + transform.forward * grabDistance;
		float distance;
		if(rightClickGrab){
			Plane plane = new Plane (new Vector3(1f,-1f,0f), position);
			if (plane.Raycast(ray, out distance)) {
				float planarLockX = grabbed.position.x;
				float planarLockZ = grabbed.position.z;
				grabbed.position = new Vector3(planarLockX,
				                               (ray.origin.y + ray.direction.y * distance)+0.25f,
				                               planarLockZ);
			}
		}
		else{
			Plane plane = new Plane (new Vector3(0,1,0),position);
			if (plane.Raycast(ray, out distance)) {
				float planarLock = grabbed.position.y;
				grabbed.position = new Vector3(ray.origin.x + ray.direction.x * distance,
			                               	   planarLock,
			                               	   ray.origin.z + ray.direction.z * distance);
			}
		}
	}
}
