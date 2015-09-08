using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.VR;

public class SubmitPressed : MonoBehaviour {
    public int newSceneNumber = 1;
    public string sliderPlayerPrefsValue = "taskTime";
    public string showTimePlayerPrefsValue = "showTime";
    public string subjectIDPlayerPrefsValue = "subjectID";
    public Slider slider;
    public Toggle displayTime;
    public InputField subjectID;

    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        if(b!=null)
            b.onClick.AddListener(delegate() { OnClick(); });
    }

    void OnClick()
    {
        Debug.Log("Submitting and Changing Scenes...");
        if (slider != null)
            PlayerPrefs.SetFloat(sliderPlayerPrefsValue, slider.value);
        if (displayTime != null)
            PlayerPrefs.SetFloat(showTimePlayerPrefsValue, displayTime.isOn?1f:0f);
        if (subjectID != null)
            PlayerPrefs.SetString(subjectIDPlayerPrefsValue, subjectID.text);
        VRSettings.enabled = true;
        Application.LoadLevel(newSceneNumber);
    }
}
