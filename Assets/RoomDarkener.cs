using UnityEngine;
using System.Collections;

public class RoomDarkener : MonoBehaviour {

    [Range(0,1)]
    public float lightLevel;
    private float currentLightLevel;
    private Renderer[] objectsToModify;
    private Color[] originalColors;
	// Use this for initialization
	void Start () {
        objectsToModify = (Renderer[])this.gameObject.GetComponentsInChildren<Renderer>();
        originalColors = new Color[objectsToModify.Length];
        for (int i = 0; i < objectsToModify.Length; i++)
            originalColors[i] = objectsToModify[i].material.color;
	}
	
	// Update is called once per frame
	void Update () {
        if (currentLightLevel != lightLevel)
        {
            currentLightLevel = lightLevel;
            for (int i = 0; i < objectsToModify.Length; i++)
                objectsToModify[i].material.color = Color.Lerp(originalColors[i], Color.black, currentLightLevel);
        }
	}
}
