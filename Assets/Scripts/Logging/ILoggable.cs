using UnityEngine;
using System.Collections;

public interface ILoggable {

	//For the logger to write to file
	string getObjectStateLogData ();
	//For reloading a logger state, returns unused data at end of string
	string loadObjectStateFromLogData(string logData);
}
