using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject TargetReticle;
    public GameObject MobileControlScheme;
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
        if ((InputControls.Instance.mode == InputControls.controlMode.Freeroam))
        {
            if (InputControls.Instance.testingMobile) MobileControlScheme.SetActive(true);
            TargetReticle.SetActive(true);
        }
        else
        {
            if (InputControls.Instance.testingMobile) MobileControlScheme.SetActive(false);
            TargetReticle.SetActive(false);
        }
        
            
    }
    
}
