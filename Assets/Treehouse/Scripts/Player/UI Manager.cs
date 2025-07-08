using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject TargetReticle;
    void OnEnable()
    {
        InputControls.ControlSchemeChanged += UpdateUI;
    }

    void OnDisable()
    {
        InputControls.ControlSchemeChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        if ((InputControls.Instance.mode == InputControls.controlMode.Freeroam)) TargetReticle.SetActive(true);
        else TargetReticle.SetActive(false);
    }
    
}
