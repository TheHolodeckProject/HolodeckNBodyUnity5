using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FeedbackSliderColor : MonoBehaviour {

    public Slider feedbackSlider;
    public Image highAccPanel;
    public Image lowAccPanel;

    public void UpdateSliderColor()
    {
        if (!feedbackSlider.gameObject.activeSelf) feedbackSlider.gameObject.SetActive(true);
        Color newcolor = new Color( 0f + feedbackSlider.value, 1 - (feedbackSlider.value), 0f);
        feedbackSlider.transform.Find("Fill Area").transform.Find("Fill").GetComponentInChildren<Image>().color = newcolor;
    }

    public void AccuracyMarkers()
    {
        if (feedbackSlider.value > .25f & !highAccPanel.gameObject.activeSelf)
        {
            highAccPanel.gameObject.SetActive(true);
        }

        if (feedbackSlider.value > .75f & !lowAccPanel.gameObject.activeSelf)
        {
            lowAccPanel.gameObject.SetActive(true);
        }
    }
}
