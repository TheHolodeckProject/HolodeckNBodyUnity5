using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class NBodyDemoMode : MonoBehaviour {

    public bool loadPlayerPrefs = false;

    [Header("Prefab Objects")]
    public GameObject bodyObjectPrefab;
    public GameObject trailsObjectPrefab;
    public GameObject trailObject;

    [Header("Generating Parameters")]
    public int numberOfBodies = 300;
    public Vector3 genRadii = new Vector3(0.3f, 0.3f, 0.3f);
    public Vector3 genVelocityRange = new Vector3(0f, 0f, 0f);
    public float genMinMass = 10000f;
    public float genMaxMass = 15000f;
    public bool perpendicularVelocities = true;
    public float perpendicularVelocitiesScaling = 2500000f;

    [Header("Render Properties")]
    public float renderScale = 0.1f;

    [Header("Avatar Behavior Properties")]
    public float massScaling = 100;
    public float sizeScaling = 6f;

    [Header("Environment Properties")]
    public Bounds scoreBounds = new Bounds(new Vector3(0f, 1.2f, 0f), new Vector3(3.2f, 1.15f, 2.15f));

    [Header("Controller Properties")]
    public float lookRadius = 0.25f;
    public KeyCode pauseKey = KeyCode.C;
    public KeyCode resetKey = KeyCode.Space;
    public Camera lookCamera;

    [Header("User Interface Game Objects")]
    public GameObject accuracySliderObject;
    public GameObject timerTextObject;
    public GameObject bestScoreTextObject;
    public GameObject itemsRemainingTextObject;
    public GameObject totalTaskTimeTextObject;

    //Private variables for efficiency of look response property changing
    private float startMass;
    private float lookMass;
    private Vector3 startScale;
    private Vector3 lookScale;

    //Special "trails" object private variables 
    private int trailsBodyIndex = 0; //This can be changed if for some reason it is desirable to use something other than the 0th GBody object
    private GBody trailsBody;
    private GameObject trail;
    private GameObject trailsBodyObject;
    private ParticleSystem particles;

    private bool isLooking; //Look state

    //Private UI component variables
    private Slider progressSlider;
    private Text timerText;
    private Text bestScoreText;
    private Text itemsRemainingText;
    private Text totalTaskTimeText;

    //The existing NBody Simulation
    private NBodySimulation sim;

    //The start time of each session
    private DateTime startTime;
    private DateTime restartTime;

    //A cached copy of the best score
    private float bestScore = 120f;
    private int resetCount = 0;
    private Logger log;

    //Configured time for task to run before haulting
    private float taskTime = 10f;

	// Use this for initialization
    void Start()
    {
        //Get the simulation variable
        NBodySimulation[] sims = gameObject.GetComponents<NBodySimulation>();

        if (sims == null)
        {
            Debug.LogWarning("Warning: No NBodySimulation attached to object " + gameObject.name + " which has NBodyDemoMode component; demo will not run.");
            return;
        }

        if (sims.Length != 1)
        {
            Debug.LogWarning("Warning: More than one NBodySimulation attached to object " + gameObject.name + "; using first found.");
        }

        sim = sims[0];

        //Load saved preferences
        if(loadPlayerPrefs)
            LoadPlayerPrefsValues();

        //Generate Body Objects
        GBody[] bodies = new GBody[numberOfBodies];
        for (int i = 0; i < bodies.Length; i++)
        {
            GameObject renderObject;
            if (i == trailsBodyIndex)
            {
                renderObject = (GameObject)GameObject.Instantiate(trailsObjectPrefab);
                trail = (GameObject)GameObject.Instantiate(trailObject);
                trail.transform.parent = renderObject.transform;
                trailsBodyObject = renderObject;
            }
            else renderObject = (GameObject)GameObject.Instantiate(bodyObjectPrefab);
            renderObject.transform.parent = this.transform;
            Vector3 pos = UnityEngine.Random.insideUnitSphere;
            pos.Scale(genRadii);
            Vector3 vel = UnityEngine.Random.insideUnitSphere;
            vel.Scale(genVelocityRange);
            if (perpendicularVelocities)
                vel = new Vector3(pos.z / perpendicularVelocitiesScaling, 0, -pos.x / perpendicularVelocitiesScaling);
            float mass = UnityEngine.Random.Range(genMinMass, genMaxMass);

            bodies[i] = new GBody(pos, vel, mass, renderObject, renderScale);

            if (i == trailsBodyIndex) trailsBody = bodies[i];
        }
        sim.SetGBodyObjects(bodies);

        lookCamera.gameObject.AddComponent<SimpleObjectLogger>();
        trailsBodyObject.AddComponent<SimpleObjectLogger>();

        //Configure look response parameters
        startMass = trailsBody.Mass;
        lookMass = startMass * massScaling;
        startScale = trailsBodyObject.transform.localScale;
        lookScale = trailsBodyObject.transform.localScale * sizeScaling;

        //Configure particle emitter
        particles = trailsBodyObject.GetComponent<ParticleSystem>();
        particles.enableEmission = false;
        
        //Populate UI element components
        progressSlider = accuracySliderObject.GetComponent<Slider>();
        progressSlider.maxValue = sim.GetGBodyObjects().Length;
        itemsRemainingText = itemsRemainingTextObject.GetComponent<Text>();
        timerText = timerTextObject.GetComponent<Text>();
        bestScoreText = bestScoreTextObject.GetComponent<Text>();
        totalTaskTimeText = totalTaskTimeTextObject.GetComponent<Text>();

        //Initialize High Score UI Component
        bestScoreText.text = "\r\nBest Score\r\n" + bestScore.ToString("F3") + "s";

        //Initialize System State Parameters
        isLooking = false;

        startTime = DateTime.Now;
        restartTime = DateTime.Now;

        if (PlayerPrefs.HasKey("showTime"))
        {
            bool showTime = PlayerPrefs.GetFloat("showTime") != 0;
            if (!showTime)
                totalTaskTimeTextObject.GetComponent<Text>().enabled = false;
        }
        if (PlayerPrefs.HasKey("taskTime"))
        {
            taskTime = PlayerPrefs.GetFloat("taskTime") * 60;
        }
        totalTaskTimeText.text = taskTime + "s";

        log = gameObject.GetComponent<Logger>();
        log.GenerateLoggableObjectsList();
        log.BeginLogging();
    }

    void RandomizeObjects()
    {
        GBody[] bodies = sim.GetGBodyObjects();
        foreach (GBody b in bodies)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere;
            pos.Scale(genRadii);
            Vector3 vel = UnityEngine.Random.insideUnitSphere;
            vel.Scale(genVelocityRange);
            if (perpendicularVelocities)
                vel = new Vector3(pos.z / perpendicularVelocitiesScaling, 0, -pos.x / perpendicularVelocitiesScaling);
            float mass = UnityEngine.Random.Range(genMinMass, genMaxMass);
            b.Position = pos;
            b.Velocity = vel;
            b.Mass = mass;
        }
    }

    private bool previousFrameResetKey = false;
    // Update is called once per frame
    void Update()
    {
        //Detect look distance
        Vector3 cameraCenter = lookCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, lookCamera.nearClipPlane));
        Ray r = new Ray(cameraCenter, lookCamera.transform.forward);
        float dist = Vector3.Cross(r.direction, trailsBodyObject.transform.position - r.origin).magnitude;
        if (dist < lookRadius)
        {
            if (!isLooking)
            {
                //Rising edge of looking behavior
                isLooking = true;
                iTween.ScaleTo(trailsBodyObject, lookScale, 1f);
            }
            trailsBody.Mass = lookMass;
            particles.enableEmission = true;
        }
        else
        {
            if (isLooking)
            {
                //Falling edge of looking behavior
                isLooking = false;
                iTween.ScaleTo(trailsBodyObject, startScale, 1f);
            }
            trailsBody.Mass = startMass;
            particles.enableEmission = false;
        }

        //Process reset and quit keys
        if (Input.GetKey(pauseKey))
            sim.pauseSimulation = true;
        else
            sim.pauseSimulation = false;
        bool resetKeyState = Input.GetKey(resetKey);
        if (resetKeyState && !previousFrameResetKey)
            Reset();
        previousFrameResetKey = resetKeyState;
        if (Input.GetKey(KeyCode.F5))
            ResetPlayerPrefsToDefault();
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
    }

    void FixedUpdate()
    {
        //Count objects still in simulation boundaries
        int count = 0;
        foreach (Transform child in sim.gameObject.transform)
            if (scoreBounds.Contains(child.position))
                count++;
        progressSlider.value = count;

        //Capture current time
        DateTime current = DateTime.Now;

        //Display unconditional UI components
        itemsRemainingText.text = "\r\n\r\n\r\n\r\n\r\n\r\nItems Remaining in Room : " + count.ToString("D3");
        timerText.text = (current.Subtract(restartTime).TotalMilliseconds / 1000).ToString("F3") + "s";
        double timeRemaining = (taskTime - (current.Subtract(startTime).TotalMilliseconds / 1000));
        totalTaskTimeText.text = timeRemaining.ToString("F3") + "s";
        //If victory condition is met (count == 0)
        if (count == 0)
        {
            //Record the time and check for best score
            float scoreT = (float)current.Subtract(restartTime).TotalMilliseconds / 1000f;
            if (scoreT < bestScore)
            {
                bestScore = scoreT;
                bestScoreText.text = "\r\nBest Score\r\n" + bestScore.ToString("F3") + "s";
            }
            //SaveDemoValuesToPlayerPrefs();
        }

        if (timeRemaining <= 0)
        {
            Application.Quit();
            Debug.Log("Quitting");
        }
    }

    private string getTrialString()
    {
        return "Reset#: " + resetCount + "; Best Score: " + bestScore;
    }
    public void Reset()
    {
        log.Pause();
        log.FinishTrial(getTrialString());
        resetCount++;
        //Reinitialize the demo
        RandomizeObjects();
        //trailsBodyObject.GetComponent<TrailRenderer>().time = -1;
        DestroyImmediate(trail);
        trail = (GameObject)GameObject.Instantiate(trailObject);
        trail.transform.parent = trailsBodyObject.transform;
        trail.transform.localPosition = Vector3.zero;
        log.Resume();
        restartTime = DateTime.Now;
    }

    void OnApplicationQuit()
    {
        try { log.FinishTrial(getTrialString()); log.Finish(); }
        catch (Exception) { }
    }

    void LoadPlayerPrefsValues()
    {
        //Make sure there are some player prefs values that could be loaded; if there aren't, generate the defaults
        if (!PlayerPrefs.HasKey("numberOfBodies"))
            ResetPlayerPrefsToDefault();

        numberOfBodies = PlayerPrefs.GetInt("numberOfBodies");
        genRadii = new Vector3(PlayerPrefs.GetFloat("genRadii.x"), PlayerPrefs.GetFloat("genRadii.y"), PlayerPrefs.GetFloat("genRadii.z"));
        genVelocityRange = new Vector3(PlayerPrefs.GetFloat("genVelocityRange.x"), PlayerPrefs.GetFloat("genVelocityRange.y"), PlayerPrefs.GetFloat("genVelocityRange.z"));
        genMinMass = PlayerPrefs.GetFloat("genMinMass");
        genMaxMass = PlayerPrefs.GetFloat("genMaxMass");
        perpendicularVelocities = PlayerPrefs.GetInt("perpendicularVelocities") != 0;
        perpendicularVelocitiesScaling = PlayerPrefs.GetFloat("perpendicularVelocitiesScaling");

        renderScale = PlayerPrefs.GetFloat("renderScale");

        massScaling = PlayerPrefs.GetFloat("massScaling");
        sizeScaling = PlayerPrefs.GetFloat("sizeScaling");

        //Not saving boundary as it can't be changed without scene modification

        lookRadius = PlayerPrefs.GetFloat("lookRadius");

        //NBody Simulation Preferences
        sim.maxVelocity = PlayerPrefs.GetFloat("sim.maxVelocity");
        sim.G = PlayerPrefs.GetFloat("sim.G");
        sim.timestep = PlayerPrefs.GetFloat("sim.timestep");

        //Best Score
        bestScore = PlayerPrefs.GetFloat("bestScore");
    }

    void SaveDemoValuesToPlayerPrefs()
    {
        PlayerPrefs.SetInt("numberOfBodies", numberOfBodies);
        PlayerPrefs.SetFloat("genRadii.x", genRadii.x);
        PlayerPrefs.SetFloat("genRadii.y", genRadii.y);
        PlayerPrefs.SetFloat("genRadii.z", genRadii.z);
        PlayerPrefs.SetFloat("genVelocityRange.x", genVelocityRange.x);
        PlayerPrefs.SetFloat("genVelocityRange.y", genVelocityRange.y);
        PlayerPrefs.SetFloat("genVelocityRange.z", genVelocityRange.z);
        PlayerPrefs.SetFloat("genMinMass", genMinMass);
        PlayerPrefs.SetFloat("genMaxMass", genMaxMass);
        PlayerPrefs.SetInt("perpendicularVelocities", perpendicularVelocities ? 0 : 1);
        PlayerPrefs.SetFloat("perpendicularVelocitiesScaling", perpendicularVelocitiesScaling);

        PlayerPrefs.SetFloat("renderScale", renderScale);

        PlayerPrefs.SetFloat("massScaling", massScaling);
        PlayerPrefs.SetFloat("sizeScaling", sizeScaling);

        //Not saving boundary as it can't be changed without scene modification

        PlayerPrefs.SetFloat("lookRadius", lookRadius);
                
        //NBody Simulation Preferences
        PlayerPrefs.SetFloat("sim.maxVelocity", sim.maxVelocity);
        PlayerPrefs.SetFloat("sim.G", sim.G);
        PlayerPrefs.SetFloat("sim.timestep", sim.timestep);

        //Best Score
        PlayerPrefs.SetFloat("bestScore", bestScore);
    }

    public static void ResetPlayerPrefsToDefault()
    {
        PlayerPrefs.SetInt("numberOfBodies", 300);
        PlayerPrefs.SetFloat("genRadii.x", 0.3f);
        PlayerPrefs.SetFloat("genRadii.y", 0.3f);
        PlayerPrefs.SetFloat("genRadii.z", 0.3f);
        PlayerPrefs.SetFloat("genVelocityRange.x", 0f);
        PlayerPrefs.SetFloat("genVelocityRange.y", 0f);
        PlayerPrefs.SetFloat("genVelocityRange.z", 0f);
        PlayerPrefs.SetFloat("genMinMass", 10000f);
        PlayerPrefs.SetFloat("genMaxMass", 15000f);
        PlayerPrefs.SetInt("perpendicularVelocities", 1);
        PlayerPrefs.SetFloat("perpendicularVelocitiesScaling", 2500000f);

        PlayerPrefs.SetFloat("renderScale", 0.001f);

        PlayerPrefs.SetFloat("massScaling", 100f);
        PlayerPrefs.SetFloat("sizeScaling", 6f);

        //Not saving boundary as it can't be changed without scene modification

        PlayerPrefs.SetFloat("lookRadius", 0.25f);

        //NBody Simulation Preferences
        PlayerPrefs.SetFloat("sim.maxVelocity", 0.001f);
        PlayerPrefs.SetFloat("sim.G", 6.67384e-20f);
        PlayerPrefs.SetFloat("sim.timestep", 10000f);

        //Best Score
        PlayerPrefs.SetFloat("bestScore", 120f);
    }
}
