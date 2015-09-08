using UnityEngine;
using System.Collections;
using System;
using System.IO;
//using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class Logger : MonoBehaviour {

	private ILoggable[] loggableObjects; //This collection contains objects whose state should be logged on Update
	private const string dateTimeFormat = "HH_mm_ss_dd-MM-yyyy"; //This string represents the DateTime output format for the filename
    private const string subfolderName = "Logged_Data";
	private StreamWriter rawWriter = null; //This writer is used to write to file
	private StreamWriter summaryWriter = null;
	private string currentFileTimestamp = "";
	private string subID;
	string firstTickOutput = "";
	string lastTickOutput = "";
	string previousTickOutput = "";
	private bool paused = false;

	public void Pause(){
		paused = true;
	}
	public void Resume(){
		paused = false;
	}
	
	// Update is called once per frame
	public void Update () {
				if (!(loggableObjects == null || loggableObjects.Length <= 0 || rawWriter == null) && !paused) {
						//try{
                            //Write a timestamp for data stability
						    string timestamp = DateTime.Now.ToBinary () + "";
						    rawWriter.WriteLine (timestamp);
						    //Output all object information
						    StringBuilder tickOutputBuilder = new StringBuilder();
						    for (int i = 0; i < loggableObjects.Length; i++) {
                                tickOutputBuilder.Append(loggableObjects[i].getObjectStateLogData());
							    tickOutputBuilder.Append ("\r\n");
						    }
						    string tickOutput = tickOutputBuilder.ToString ();
						    if(!tickOutput.Equals(previousTickOutput)){
							    if(firstTickOutput=="")
								    firstTickOutput = timestamp+"\r\n"+tickOutput;
							    rawWriter.Write(tickOutput);
							    lastTickOutput = timestamp+"\r\n"+tickOutput;
						    }
						    previousTickOutput = tickOutput;
                        //} catch (MissingReferenceException) { }
				}
		}

	public void GenerateLoggableObjectsList(){
		//Get the list of objects - this is a one-time function. If new objects are created, there is currently no way to log them without creating a new logger.
		List<ILoggable> logObjs = new List<ILoggable> ();
		GameObject[] objs = (GameObject[])FindObjectsOfType (typeof(GameObject));
		//Debug.Log ("Searching " + objs.Length + " GameObject objects for ILoggable interfaces.");
		for (int i = 0; i < objs.Length; i++) {
			List<ILoggable> logScripts = new List<ILoggable> ();
			GetInterfaces<ILoggable> (out logScripts, objs [i]);
			if (logScripts.Count > 0)
				logObjs.AddRange (logScripts);
		}
		List<ILoggable> output = new List<ILoggable> ();
		for (int i = 0; i < logObjs.Count; i++)
						if (logObjs [i] != null)
								output.Add (logObjs [i]);
		loggableObjects = output.ToArray ();
	}

	public void BeginLogging(){
        subID = PlayerPrefs.GetString("subjectID");
        string dir1 = Application.dataPath.Replace('/', '\\') + "\\" + subfolderName + "\\" + subID + "\\";
        Directory.CreateDirectory(dir1);

		string substring = ("Sub" + subID);

		GenerateLoggableObjectsList ();

		//Debug.Log ("Found " + loggableObjects.Length + " ILoggable objects.");
		
		//Create the appropriate filename given the options
        string rawFilename = dir1 + "RawLog.csv";
        Debug.LogWarning(rawFilename);
		currentFileTimestamp = DateTime.Now.ToString (dateTimeFormat);
		rawFilename = appendTextToFilename (rawFilename,substring);
		rawFilename = appendTextToFilename (rawFilename,currentFileTimestamp);
		
		//Create the file writer
		rawWriter = new StreamWriter (rawFilename, false);
		rawWriter.AutoFlush = true;

		//Create the appropriate filename given the options
        string summaryFilename = dir1 + "SummaryLog.csv";
		summaryFilename = appendTextToFilename (summaryFilename,substring);
		summaryFilename = appendTextToFilename (summaryFilename,currentFileTimestamp);

		//Create the file writer
		summaryWriter = new StreamWriter (summaryFilename);
        summaryWriter.WriteLine("TrialDescriptor,Stimulus,BlobNum,StartPosX,StartPosY,StartPosZ,EndPosX,EndPosY,EndPosZ,StartRotX,StartRotY,StartRotZ,EndRotX,EndRotY,EndRotZ");
	}

	public void FinishTrial(string trialDescriptor){
		string[] trialLines = firstTickOutput.Split (new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		string[] trialLinesEnd = lastTickOutput.Split (new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 1; i < trialLines.Length; i++) {
			string[] lineSplit = trialLines[i].Split (new string[]{":",","},StringSplitOptions.None);
			string[] endLineSplit = trialLinesEnd[i].Split(new string[]{":",","},StringSplitOptions.None);
            summaryWriter.Write(trialDescriptor + ",");
			summaryWriter.Write ((i-1)+","+lineSplit[0]+",");
			summaryWriter.Write (lineSplit[1]+","+lineSplit[2]+","+lineSplit[3]+",");
			summaryWriter.Write (endLineSplit[1]+","+endLineSplit[2]+","+endLineSplit[3]+",");
			summaryWriter.Write (lineSplit[4]+","+lineSplit[5]+","+lineSplit[6]+",");
			summaryWriter.WriteLine (endLineSplit[4]+","+endLineSplit[5]+","+endLineSplit[6]);
		}
		firstTickOutput = "";
		lastTickOutput = "";
		previousTickOutput = "";
        rawWriter.WriteLine("End of Trial " + trialDescriptor);
        summaryWriter.Flush();
        rawWriter.Flush();
	}

	public void Finish(){
		rawWriter.WriteLine ("End of File");
		rawWriter.Close ();
		summaryWriter.Close ();
	}

	//When quitting, attempt to close the streams
	void OnApplicationQuit() {
		try{rawWriter.Close ();}catch(Exception){}
		try{summaryWriter.Close ();}catch(Exception){}
	}

	private string appendTextToFilename(string filename, string text){
		string[] fileTokens = filename.Split(new string[]{"."},System.StringSplitOptions.None);
		string output = "";
		for (int i = 0; i < fileTokens.Length - 1; i++)
			output += fileTokens [i] + "_";
		return output + text + "." + fileTokens [fileTokens.Length - 1];
	}

	public static void GetInterfaces<T>(out List<T> resultList, GameObject objectToSearch) where T: class {
		MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
		resultList = new List<T>();
		foreach(MonoBehaviour mb in list){
			if(mb is T){
				//found one
				resultList.Add((T)((System.Object)mb));
			}
		}
	}
}
