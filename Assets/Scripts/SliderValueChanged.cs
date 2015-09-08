using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderValueChanged : MonoBehaviour {

    private Slider referenceSlider;
    private Text sliderTextDisplay;
    public float maxInMinutes = 10f;
    public float minInMinutes = 1f;
    public string sliderPlayerPrefsValue = "taskTimeInMinutes";
    public string sliderTextDisplayFormat = "Task Time: {0,3:00.0} minutes";
	// Use this for initialization
	void Start () {
        referenceSlider = this.gameObject.GetComponent<Slider>();
        if (referenceSlider != null)
        {
            referenceSlider.minValue = minInMinutes;
            referenceSlider.maxValue = maxInMinutes;
        }
        sliderTextDisplay = gameObject.GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (referenceSlider != null)
        {
            float sliderValue = referenceSlider.value;
            string displayValue = string.Format(sliderTextDisplayFormat, sliderValue);
            if (sliderTextDisplay != null)
                sliderTextDisplay.text = displayValue;
        }
	}
}
