using UnityEngine;
using System.Collections;
using System.Text;

public class SimpleObjectLogger : MonoBehaviour, ILoggable {

	//For the logger to write to file
	public virtual string getObjectStateLogData ()
	{
		StringBuilder builder = new StringBuilder ();
		builder.Append (gameObject.name.Replace ("(Clone)",""));
		builder.Append (':');
		builder.Append(gameObject.transform.position.x);
		builder.Append(',');
		builder.Append(gameObject.transform.position.y);
		builder.Append(',');
		builder.Append(gameObject.transform.position.z);
		builder.Append(',');
		
		builder.Append(gameObject.transform.rotation.x);
		builder.Append(',');
		builder.Append(gameObject.transform.rotation.y);
		builder.Append(',');
		builder.Append(gameObject.transform.rotation.z);
		builder.Append(',');
		builder.Append(gameObject.transform.rotation.w);
		builder.Append(',');
		
		builder.Append(gameObject.transform.localScale.x);
		builder.Append(',');
		builder.Append(gameObject.transform.localScale.y);
		builder.Append(',');
		builder.Append(gameObject.transform.localScale.z);
		
		return builder.ToString();
	}
	
	//For reloading a logger state
	public virtual string loadObjectStateFromLogData(string logData)
	{
		string[] logDataTokens = logData.Split (new string[]{","}, System.StringSplitOptions.None);
		
		Vector3 position = new Vector3 (float.Parse (logDataTokens [0]), float.Parse (logDataTokens [1]), float.Parse (logDataTokens [2]));
		Quaternion rotation = new Quaternion (float.Parse (logDataTokens [3]), float.Parse (logDataTokens [4]), float.Parse (logDataTokens [5]), float.Parse (logDataTokens [6]));
		Vector3 localScale = new Vector3 (float.Parse (logDataTokens [7]), float.Parse (logDataTokens [8]), float.Parse (logDataTokens [9]));
		
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		gameObject.transform.localScale = localScale;

		StringBuilder remainingStringBuilder = new StringBuilder ();
		//If there are more tokens than we used
		if (logDataTokens.Length > 9) {
			//Append any tokens other than the last token
			for (int i = 10; i < logDataTokens.Length - 1; i++) {
				remainingStringBuilder.Append (logDataTokens [i] + ",");
			}
			//Append the last token
			remainingStringBuilder.Append (logDataTokens[logDataTokens.Length-1]);
		}

		//Return the remaining tokens to be processed by inherited classes
		return remainingStringBuilder.ToString ();
	}
}
