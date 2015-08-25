using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class FullVROverlayUI : MonoBehaviour {

    //Calls the positionSync function every frame
    void Update() { positionSync(); }

    public Vector3 positionOffset = new Vector3(0f, 0.5f, 0.6f);

    void positionSync()
    {
        //Finds a stable point on the body, near the collarbone
        GameObject center = GameObject.Find("SpineShoulder");
        //Moves the in-game Oculus camera to that position
        if (center != null)
        {
            Vector3 position = new Vector3(center.transform.position.x + positionOffset.x, center.transform.position.y + positionOffset.y, center.transform.position.z + positionOffset.z);
            //!!! - Replaced this line to find the player instead of specifically the OVRPlayerController, as I don't think we should be using it
            //GameObject.Find("OVRPlayerController").transform.position = position;
            GameObject.Find("OVRCameraController").transform.position = position;
        }
    }
}
