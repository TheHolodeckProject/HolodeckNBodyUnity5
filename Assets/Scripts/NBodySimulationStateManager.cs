using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class NBodySimulationStateManager : MonoBehaviour {
    public GameObject NBodySimulationObject;
    public GameObject NBodySimulationPrefab;
    public GameObject PlayerObject;
    public GameObject VelocityMarkerObject;
    public GameObject TargetPrefab;
    public int maxNumberOfRounds = 3;
    public float roundDurationInMilliseconds = 30000;
    public Vector4 targetLocationRangeXYZR;

    public GameObject PlayerPlacementBoundry;
    public GameObject TargetPlacementBoundry;

    public AudioClip[] instructionAudioClips;
    public AudioSource instructionAudioSource;
    public GameObject instructionArrowMarkerPrefab;
    public GameObject instructionArrowMarkerObject;

    //Variables for config viewing only
    public float currentScore;
    public float scoreMin;
    public NBodySimulationState state;

    private GameObject targetInstance;

    public KeyCode nextKey = KeyCode.Space;
    public GameObject nextButtonManagerObject;

    public bool PlayFirstTimeInstructions = true;

    private Vector3 targetLoc;
    private List<float> scoreHistory;
    public int numberOfRoundsCompleted;
    private Stopwatch stopwatch;
    public enum NBodySimulationState { Initialize=0, FirstTimeInstructions, FTIWelcome, FTIGoal, FTITarget, FTISpaceship, FTITrajectory, FTITime, FTIScore, FTIScorePolarity, FTIWait,
        InitializeNewRound, ResetObjectPositions, WaitForShipPlacementTrigger, WaitForVelocityMarkerPlacementTrigger, WaitForRoundStartTrigger, WaitForRoundToComplete, ScoreRound, WaitForContinueTrigger, Shutdown}

    public GameObject textDisplayObject;
    public GameObject accuracyFeedbackSlider;
    public GameObject manualLoggerObject;
    private ManualLogger logger;
	// Use this for initialization
	void Start () {
        state = NBodySimulationState.Initialize;
        if(PlayerPrefs.HasKey("NBodyNumberOfRounds"))
            maxNumberOfRounds = PlayerPrefs.GetInt("NBodyNumberOfRounds");

        logger = manualLoggerObject.GetComponent<ManualLogger>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        switch (state)
        {
            case NBodySimulationState.Initialize:
                scoreHistory = new List<float>();
                numberOfRoundsCompleted = 0;
                targetInstance = null;
                logger.BeginLogging("NBody", ".dat");
                if (PlayFirstTimeInstructions)
                    state = NBodySimulationState.FirstTimeInstructions;
                else
                    state = NBodySimulationState.InitializeNewRound;
                break;
            case NBodySimulationState.FirstTimeInstructions:
                instructionAudioSource.clip = instructionAudioClips[4];
                instructionAudioSource.Play();

                if (targetInstance != null)
                        DestroyObject(targetInstance);
                targetLoc = new Vector3(targetLocationRangeXYZR.w/2, targetLocationRangeXYZR.w/2, targetLocationRangeXYZR.w/2);
                targetInstance = (GameObject)GameObject.Instantiate(TargetPrefab);
                targetInstance.transform.parent = NBodySimulationObject.transform;
                targetInstance.transform.localPosition = targetLoc;

                PlayerObject.transform.localPosition = Vector3.zero;
                VelocityMarkerObject.transform.localPosition = new Vector3(0f, 0f, 0.2f);

                state = NBodySimulationState.FTIWelcome;
                break;
            case NBodySimulationState.FTIWelcome:
                if (!instructionAudioSource.isPlaying)
                {
                    instructionAudioSource.clip = instructionAudioClips[5];
                    instructionAudioSource.Play();
                    state = NBodySimulationState.FTIGoal;
                }
                break;
            case NBodySimulationState.FTIGoal:
                if (!instructionAudioSource.isPlaying)
                {
                    instructionAudioSource.clip = instructionAudioClips[6];
                    instructionAudioSource.Play();

                    instructionArrowMarkerObject = (GameObject)GameObject.Instantiate(instructionArrowMarkerPrefab, new Vector3(targetInstance.transform.position.x, targetInstance.transform.position.y + 0.1f, targetInstance.transform.position.z), Quaternion.identity);

                    state = NBodySimulationState.FTITarget;
                }
                break;
            case NBodySimulationState.FTITarget:
                if (!instructionAudioSource.isPlaying)
                {
                    instructionAudioSource.clip = instructionAudioClips[7];
                    instructionAudioSource.Play();

                    instructionArrowMarkerObject.GetComponent<VerticalBounce>().SetNewPosition(new Vector3(PlayerObject.transform.position.x, PlayerObject.transform.position.y + 0.1f, PlayerObject.transform.position.z));

                    state = NBodySimulationState.FTISpaceship;
                }
                break;
            case NBodySimulationState.FTISpaceship:
                if (!instructionAudioSource.isPlaying)
                {
                    instructionAudioSource.clip = instructionAudioClips[8];
                    instructionAudioSource.Play();

                    instructionArrowMarkerObject.GetComponent<VerticalBounce>().SetNewPosition(new Vector3(VelocityMarkerObject.transform.position.x, VelocityMarkerObject.transform.position.y + 0.1f, VelocityMarkerObject.transform.position.z));

                    state = NBodySimulationState.FTITrajectory;
                }
                break;
            case NBodySimulationState.FTITrajectory:
                if (!instructionAudioSource.isPlaying)
                {
                    instructionAudioSource.clip = instructionAudioClips[9];
                    instructionAudioSource.Play();

                    state = NBodySimulationState.FTITime;
                }
                break;
            case NBodySimulationState.FTITime:
                if (!instructionAudioSource.isPlaying)
                {
                    instructionAudioSource.clip = instructionAudioClips[10];
                    instructionAudioSource.Play();
                    state = NBodySimulationState.FTIScore;
                }
                break;
            case NBodySimulationState.FTIScore:
                if (!instructionAudioSource.isPlaying)
                {
                    instructionAudioSource.clip = instructionAudioClips[11];
                    instructionAudioSource.Play();
                    state = NBodySimulationState.FTIScorePolarity;
                }
                break;
            case NBodySimulationState.FTIScorePolarity:
                if (!instructionAudioSource.isPlaying)
                {
                    instructionAudioSource.clip = instructionAudioClips[2];
                    instructionAudioSource.Play();

                    instructionArrowMarkerObject.GetComponent<VerticalBounce>().SetNewPosition(new Vector3(nextButtonManagerObject.transform.position.x, nextButtonManagerObject.transform.position.y + 0.1f, nextButtonManagerObject.transform.position.z));

                    state = NBodySimulationState.FTIWait;
                }
                break;
            case NBodySimulationState.FTIWait:
                if (userStateAdvance())
                {
                    DestroyObject(instructionArrowMarkerObject);
                    state = NBodySimulationState.InitializeNewRound;
                }
                break;
            case NBodySimulationState.InitializeNewRound:
                PlayerObject.transform.parent = PlayerPlacementBoundry.transform;
                VelocityMarkerObject.transform.parent = TargetPlacementBoundry.transform;
                VelocityMarkerObject.SetActive(true);
                PlayerPlacementBoundry.SetActive(true);
                TargetPlacementBoundry.SetActive(true);
                PlayerPlacementBoundry.GetComponent<MeshRenderer>().enabled= true;
                TargetPlacementBoundry.GetComponent<MeshRenderer>().enabled = false;
                PlayerObject.GetComponent<TrailRenderer>().enabled = false;
                GameObject.Destroy(NBodySimulationObject);
                NBodySimulationObject = (GameObject)GameObject.Instantiate(NBodySimulationPrefab);
                NBodySimulationObject.GetComponent<NBodySimulation>().playerPrefab = PlayerObject;
                NBodySimulationObject.GetComponent<NBodySimulation>().playerTarget = VelocityMarkerObject;
                instructionAudioSource.clip = instructionAudioClips[0];
                instructionAudioSource.Play();

                if(targetInstance != null)
                    DestroyObject(targetInstance);
                targetLoc = (UnityEngine.Random.insideUnitSphere * targetLocationRangeXYZR.w) + new Vector3(targetLocationRangeXYZR.x, targetLocationRangeXYZR.y, targetLocationRangeXYZR.z);
                targetInstance = (GameObject)GameObject.Instantiate(TargetPrefab);
                targetInstance.transform.parent = NBodySimulationObject.transform;
                targetInstance.transform.localPosition = targetLoc;
                scoreMin = float.MaxValue;

                stopwatch = new Stopwatch();

                state = NBodySimulationState.ResetObjectPositions;
                break;
            case NBodySimulationState.ResetObjectPositions:
                PlayerObject.transform.localPosition = Vector3.zero;
                VelocityMarkerObject.transform.localPosition = new Vector3(0f, 0f, 0.2f);
                state = NBodySimulationState.WaitForShipPlacementTrigger;
                break;
            case NBodySimulationState.WaitForShipPlacementTrigger:
                if (userStateAdvance())
                {
                    PlayerPlacementBoundry.GetComponent<MeshRenderer>().enabled = false;
                    TargetPlacementBoundry.GetComponent<MeshRenderer>().enabled = true;

                    instructionAudioSource.clip = instructionAudioClips[1];
                    instructionAudioSource.Play();

                    state = NBodySimulationState.WaitForVelocityMarkerPlacementTrigger;
                }
               break;
            case NBodySimulationState.WaitForVelocityMarkerPlacementTrigger:
               if (userStateAdvance())
               {
                   PlayerPlacementBoundry.GetComponent<MeshRenderer>().enabled = false;
                   TargetPlacementBoundry.GetComponent<MeshRenderer>().enabled = false;

                   instructionAudioSource.clip = instructionAudioClips[2];
                   instructionAudioSource.Play();

                   state = NBodySimulationState.WaitForRoundStartTrigger;
               }
               break;
            case NBodySimulationState.WaitForRoundStartTrigger:
                if (userStateAdvance())
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    PlayerPlacementBoundry.SetActive(false);
                    TargetPlacementBoundry.SetActive(false);

                    TrainRendererExtensions.Reset(PlayerObject.GetComponent<TrailRenderer>(), this);
                    PlayerObject.GetComponent<TrailRenderer>().enabled = true;
                    NBodySimulationObject.GetComponent<NBodySimulation>().StartSimulation();
                    state = NBodySimulationState.WaitForRoundToComplete;
                }
                break;
            case NBodySimulationState.WaitForRoundToComplete:
                currentScore = Vector3.Distance(PlayerObject.transform.localPosition, targetLoc);
                if (currentScore < scoreMin) scoreMin = currentScore;
                accuracyFeedbackSlider.GetComponent<UnityEngine.UI.Slider>().value = (stopwatch.ElapsedMilliseconds/roundDurationInMilliseconds);
                textDisplayObject.GetComponent<UnityEngine.UI.Text>().text = "Current Distance: " + currentScore.ToString("0.0000") + "\nCurrent Score: " + scoreMin.ToString("0.0000");
                if (stopwatch.ElapsedMilliseconds >= roundDurationInMilliseconds)
                {
                    stopwatch.Stop();
                    state = NBodySimulationState.ScoreRound;
                }
                break;
            case NBodySimulationState.ScoreRound:
                scoreHistory.Add(scoreMin);
                numberOfRoundsCompleted++;
                NBodySimulationObject.GetComponent<NBodySimulation>().pauseSimulation = true;
                UnityEngine.Debug.Log("Round: " + numberOfRoundsCompleted + ", Score: " + scoreMin);

                logger.Write("Round: " + numberOfRoundsCompleted + ", Score: " + scoreMin + "\n");

                instructionAudioSource.clip = instructionAudioClips[3];
                instructionAudioSource.Play();

                state = NBodySimulationState.WaitForContinueTrigger;
                break;
            case NBodySimulationState.WaitForContinueTrigger:
                if (userStateAdvance())
                {
                    if (numberOfRoundsCompleted < maxNumberOfRounds)
                        state = NBodySimulationState.InitializeNewRound;
                    else
                    {
                        instructionAudioSource.clip = instructionAudioClips[12];
                        instructionAudioSource.Play();
                        state = NBodySimulationState.Shutdown;
                    }
                }
                break;
            case NBodySimulationState.Shutdown:
                logger.Finish();
                //Do whatever shutdown procedure may be appropriate
                break;
        }
	}

    private bool userStateAdvance()
    {
        if (Input.GetKeyUp(nextKey)) return true;
        //TODO: Add Button Conditional Code
        return false;
    }

    //TODO: Add Button State Change Code (and integrate into state machine)
    private void SetButtonState(bool on)
    {
        if (on)
        {

        }
        else
        {

        }
    }
}

public static class TrainRendererExtensions
{
    /// <summary>
    /// Reset the trail so it can be moved without streaking
    /// </summary>
    public static void Reset(this TrailRenderer trail, MonoBehaviour instance)
    {
        instance.StartCoroutine(ResetTrail(trail));
    }

    /// <summary>
    /// Coroutine to reset a trail renderer trail
    /// </summary>
    /// <param name="trail"></param>
    /// <returns></returns>
    static IEnumerator ResetTrail(TrailRenderer trail)
    {
        var trailTime = trail.time;
        trail.time = 0;
        yield return 0;
        trail.time = trailTime;
    }
}
