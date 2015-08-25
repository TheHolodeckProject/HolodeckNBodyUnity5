using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class BinaryObjectLogger : MonoBehaviour, ILoggable {
	
	//For the logger to write to file
	public virtual string getObjectStateLogData ()
	{
		StringBuilder builder = new StringBuilder ();
		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.position.x)));
		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.position.y)));
		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.position.z)));

		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.rotation.x)));
		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.rotation.y)));
		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.rotation.z)));
		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.rotation.w)));

		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.localScale.x)));
		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.localScale.y)));
		builder.Append(Encoding.ASCII.GetString (BitConverter.GetBytes (gameObject.transform.localScale.z)));
		
		return builder.ToString();
	}
	
	//For reloading a logger state
	public virtual string loadObjectStateFromLogData(string logData)
	{
		byte[] logDataBytes = Encoding.ASCII.GetBytes (logData);
		float[] values = new float[10];
		for (int i = 0; i < values.Length; i++)
			values[i] = BitConverter.ToSingle(logDataBytes,i*4);
		
		Vector3 position = new Vector3 (values [0], values [1], values [2]);
		Quaternion rotation = new Quaternion (values [3], values [4], values [5], values [6]);
		Vector3 localScale = new Vector3 (values [7], values [8], values [9]);
		
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		gameObject.transform.localScale = localScale;
		
		//Return the remaining bytes to be processed by inherited classes
		return logData.Substring (values.Length * 4);
	}
}
