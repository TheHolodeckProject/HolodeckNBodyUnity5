using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class ManualLogger : MonoBehaviour {
    private const string dateTimeFormat = "HH_mm_ss_dd-MM-yyyy"; //This string represents the DateTime output format for the filename
    private StreamWriter writer = null; //This writer is used to write to file
    private string currentFileTimestamp = "";
    private string filename;
    private int subID = -1;
	// Use this for initialization
	void Start () {
        
	}

    public void BeginLogging(string logName, string logExtension)
    {
        DateTime currentTime = DateTime.Now;

        string rawFilename = (logName == "" ? "raw" : logName) + (logExtension == "" ? "" : (logExtension[0] == '.' ? logExtension : "." + logExtension));
        currentFileTimestamp = currentTime.ToString(dateTimeFormat);
        string substring = ("Sub" + currentFileTimestamp);

        if (PlayerPrefs.HasKey("Subject Identifier"))
        {
            subID = PlayerPrefs.GetInt("Subject Identifier");
            substring = ("Sub" + subID);
        }

        //Create the appropriate filename given the options

        filename = appendTextToFilename(rawFilename, substring);
        filename = appendTextToFilename(rawFilename, currentFileTimestamp);

        //Create the file writer
        writer = new StreamWriter(filename, false);
        writer.AutoFlush = true;
    }

    public void Write(string text)
    {
        writer.Write(text);
    }

    public void Finish()
    {
        writer.Close();
    }

    private string appendTextToFilename(string filename, string text)
    {
        string[] fileTokens = filename.Split(new string[] { "." }, System.StringSplitOptions.None);
        string output = "";
        for (int i = 0; i < fileTokens.Length - 1; i++)
            output += fileTokens[i] + "_";
        return output + text + "." + fileTokens[fileTokens.Length - 1];
    }
}
