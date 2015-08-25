using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using System;

public class HolodeckStimulusManager : MonoBehaviour
{
    public int numberOfStimuli = 3; //Static value representing number of stimuli for task
    public float Transparency = .9f; //How transparent the stimuli are (0 = invisible, 1 = opaque)
    private int[] colorNums; //A to-be-randomized list of numbers
    //private GameObject blob;
    private Texture[] colors;
    private int stimNum;
    private GameObject[] stimuli;
    ////Reset positions form a line between points P0 and P1 with even distribution (starting at end points)
    private Vector3 resetPositionP0 = new Vector3(-4f, 0f, -8f); //Position for obj0
    private Vector3 resetPositionP1 = new Vector3(4f, 0f, -8f); //Position for objN
    //Random Bounds determines the 3D box which bounds the possible random positions of each object
    //The point P0 determines one corner of the box while the point P1 determines the opposite corner
    private Vector3 randomBoundsP0 = new Vector3(-2f, 1f, -10f); //One corner of the bounding box being used
    private Vector3 randomBoundsP1 = new Vector3(1f, 4f, -7f); //The opposite corner of the bounding box being used

    /// /////////////////////////////////////////
    /// INITIALIZATION CODE /////////////////////
    /// /////////////////////////////////////////

    void Start()
    {

        //Generate timestamp 

        // Initializes an array of GameObjects of the appropriate length
        stimuli = new GameObject[numberOfStimuli];
    }
  
    /// /////////////////////////////////////////
    /// TEST PROCEDURE CODE /////////////////////
    /// /////////////////////////////////////////

    private int phase = 0;
    private bool runOncePhase1;
    private bool runOncePhase2;
    void Update()
    {
        // ??? Any difference between multiple if statements and using if else?

        //If it's in the setup phase
        if (phase == 0)
        {
            //Resets the flags so it can run through the phases properly
            runOncePhase1 = false;
            runOncePhase2 = false;
            //Picks random blobs, so that it never picks the same one twice in the same trial
            //Generates the blobs with appropriate shaders and colors
            generateRandomStimuli();
            //Generates random locations within specified bounds, so that they never overlap
            generateRandomPositions();
            //for (int i = 0; i < numberOfStimuli; i++)
            //{
            //    stimuli[i].gameObject.tag = "Untagged";
            //}
            phase = 1;
        }

        // Study Phase
        if (phase == 1)
        {
            if (runOncePhase1 == false)
            {
                Debug.Log("Entering study phase");
                //For every stimulus
                for (int i = 0; i < numberOfStimuli; i++)
                {
                    //Changes the tags of the parent objects to grabbable, and the mesh children to untagged
                    stimuli[i].gameObject.tag = "Grabbable";
                    stimuli[i].gameObject.transform.Find("Meshes").tag = "Untagged";
                    runOncePhase1 = true;
                }
            }
        }

        // Recall Phase
        if (phase == 2)
        {
            if (runOncePhase2==false)
            {
            Debug.Log("Entering recall phase");
            //For every stimulus
            for (int i = 0; i < numberOfStimuli; i++)
            {    
                //Resets the object positions
                stimuli[i].transform.localPosition = Vector3.Lerp(resetPositionP0, resetPositionP1, (((float)i) * (1f / ((float)stimuli.Length))));
            }
            runOncePhase2 = true;
            }
        }


    }
    public void HolodeckTrialReset() { phase = 0; }
    public void HolodeckStimulusReset() { phase = 2; }

      private void generateRandomStimuli()
    {
        if (stimuli != null)
            for (int i = 0; i < stimuli.Length; i++)
            {
                try
                {
                    DestroyImmediate(stimuli[i]);
                }
                catch (UnityException)
                {
                }
            }
        //Creates a list of integers, 1 for each color
        //Create a knuth shuffle index list of random indicies within the range of possible colors
        int[] stimNums = new int[100];
        for (int i = 0; i < 100; i++)
            stimNums[i] = i;
        for (int i = 0; i < numberOfStimuli; i++)
        {
            int index = UnityEngine.Random.Range(i, 100 - 1);
            int tmp = stimNums[index];
            stimNums[index] = stimNums[i];
            stimNums[i] = tmp;
        }
        //Initializes an list to hold textures created 
        // ??? Lists vs Arrays
        List<Texture> colors = new List<Texture>();
        colors.Add(Resources.Load("Red", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Blue", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Yellow", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Orange", typeof(Texture2D)) as Texture);
        colors.Add(Resources.Load("Green", typeof(Texture2D)) as Texture);
        //TO DO - Figure out how to dynamically create textures based on randomly generated RGB values (within certain ranges to only get pretty colors)
        // ??? Would it be more efficient to make a 1x1 color texture and repeat it or make a single 1000x1000 color texture?

        //Create a knuth shuffle index list of random indicies within the range of possible colors
        int[] colorNums = new int[colors.Count];
        for (int i = 0; i < colors.Count; i++)
            colorNums[i] = i;
        for (int i = 0; i < colors.Count; i++)
        {
            int index = UnityEngine.Random.Range(i, colorNums.Length - 1);
            int tmp = colorNums[index];
            colorNums[index] = colorNums[i];
            colorNums[i] = tmp;
        }
        //Actually creates the stimuli
        for (int i = 0; i < numberOfStimuli; i++)
        {
            try
            {
                ////Creates the material to be used for the blobs
                stimuli[i] = Resources.Load("Blob" + stimNums[i]) as GameObject;
                //stimuli[i].gameObject.renderer.material = new Material(Shader.Find("Custom/TransparentDiffuseWithShadow"));
                stimuli[i].gameObject.transform.Find("Meshes").GetComponent<Renderer>().material = new Material(Shader.Find("Custom/TransparentDiffuseWithShadow"));

                stimuli[i].gameObject.transform.Find("Meshes").GetComponent<Renderer>().material.mainTexture = colors[colorNums[i]];
                stimuli[i].gameObject.transform.Find("Meshes").GetComponent<Renderer>().material.color = new Color(stimuli[i].gameObject.transform.Find("Meshes").GetComponent<Renderer>().material.color.r, stimuli[i].gameObject.transform.Find("Meshes").GetComponent<Renderer>().material.color.g, stimuli[i].gameObject.transform.Find("Meshes").GetComponent<Renderer>().material.color.b, Transparency);
                //stimuli[i] = (GameObject)Instantiate(stimuli[i], stimLocation, Quaternion.identity);
                stimuli[i] = (GameObject)Instantiate(stimuli[i]);
            }
            catch (NullReferenceException) { Debug.Log("NullReferenceException while attempting to generate stimuli."); /* If there's a null reference exception it means we're missing some dependent object. This shouldn't happen, but as at least once. Keep an eye on it. */ }
        }
    }

    private int numRetries = 100;
    private void generateRandomPositions()
    {
        //Position stimuli randomly according to settings (no overlaps)
        List<Rect> overlapCheckList = new List<Rect>();
        int retries = numRetries;
        for (int i = 0; i < numberOfStimuli; i++)
        {
            // ??? Object positions are not in the center of the objects.
            // ??? localPosition vs. global position?
            stimuli[i].transform.localPosition = new Vector3(
            UnityEngine.Random.Range(randomBoundsP0.x, randomBoundsP0.y),
            UnityEngine.Random.Range(randomBoundsP0.y, randomBoundsP1.y),
            UnityEngine.Random.Range(randomBoundsP0.z, randomBoundsP1.z));

            //Check for overlapping boxes and regenerate box location if overlap occurs
            Rect newBox = new Rect(stimuli[i].transform.localPosition.x,
                                   stimuli[i].transform.localPosition.z,
                                   stimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.x * stimuli[i].transform.localScale.x,
                                   stimuli[i].GetComponentInChildren<MeshFilter>().mesh.bounds.size.z * stimuli[i].transform.localScale.z);
            if (boxesOverlapArray(newBox, overlapCheckList))
            {
                retries--;
                if (retries <= 0)
                {
                    //Force quit and overlap to prevent hang
                    retries = numRetries;
                    overlapCheckList.Add(newBox);
                    Debug.Log("Unable to find proper placement of object. Too many objects or incorrect mesh bounds?");
                }
                else i--;
            }
            else
            {
                retries = numRetries;
                overlapCheckList.Add(newBox);
            }
        }
    }
    bool boxesOverlapArray(Rect box, List<Rect> boxArray)
    {
        for (var i = 0; i < boxArray.Count; i++)
            if (boxesOverlap(box, boxArray[i])) return true;
        return false;
    }
    //Reference: http://gamemath.com/2011/09/detecting-whether-two-boxes-overlap/
    bool boxesOverlap(Rect box0, Rect box1)
    {
        if (box0.x + box0.width < box1.x) return false;
        if (box0.x > box1.x + box1.width) return false;
        if (box0.y + box0.height < box1.y) return false;
        if (box0.y > box1.y + box1.height) return false;
        return true;
    }
}