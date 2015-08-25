using UnityEngine;
using System.Collections;
using System.Text;

public class PlayerControllerLogger : SimpleObjectLogger {

	//For the logger to write to file
	public override string getObjectStateLogData ()
	{
		StringBuilder builder = new StringBuilder (base.getObjectStateLogData());

		//Add PlayerController specific logging here

		return builder.ToString();
	}

	//For reloading a logger state
	public override string loadObjectStateFromLogData(string logData)
	{
		string remainingLogData = base.loadObjectStateFromLogData (logData);

		//Process remainingLogData here

		return remainingLogData; //Return remaining log data which was not processed

	}
}
