using UnityEngine;
using System.Collections;

// ??? Why does the button change color whenever you touch it, in any MoveTask state?
public class ButtonManager : MonoBehaviour
{

    public Material buttonOnMaterial;
    public Material buttonOffMaterial;
    private bool buttonState;

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<Renderer>().material = buttonOnMaterial;
        buttonState = true;
    }

    public void SetButtonState(bool state)
    {
        buttonState = state;
        UpdateMaterial();
    }

    public bool GetButtonState()
    {
        return buttonState;
    }

    public void ToggleButtonState()
    {
        buttonState = !buttonState;
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (buttonState)
            gameObject.GetComponent<Renderer>().material = buttonOnMaterial;
        else
            gameObject.GetComponent<Renderer>().material = buttonOffMaterial;
    }
}
