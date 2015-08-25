using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

class HelperFunctions : MonoBehaviour
{
    public static void GetInterfaces<T>(out List<T> resultList, GameObject objectToSearch) where T : class
    {
        MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
        resultList = new List<T>();
        foreach (MonoBehaviour mb in list)
        {
            if (mb is T)
            {
                //found one
                resultList.Add((T)((System.Object)mb));
            }
        }
    }

    public static void GetScriptObjectsInScene<T>(out List<T> resultList) where T : class
    {
        List<T> outObjs = new List<T>();
        GameObject[] objs = (GameObject[])FindObjectsOfType(typeof(GameObject));

        //Debug.Log ("Searching " + objs.Length + " GameObject objects for ILoggable interfaces.");
        for (int i = 0; i < objs.Length; i++)
        {
            List<T> logScripts = new List<T>();
            HelperFunctions.GetInterfaces<T>(out logScripts, objs[i]);
            if (logScripts.Count > 0)
                outObjs.AddRange(logScripts);
        }
        List<T> output = new List<T>();
        for (int i = 0; i < outObjs.Count; i++)
            if (outObjs[i] != null)
                output.Add(outObjs[i]);
        resultList = output;
    }
}
