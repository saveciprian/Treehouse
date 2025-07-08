using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    private Image buttonImg;
    private TMP_Text text;
    public void Toggle()
    {
        buttonImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        text.color = new Color(0.3f, 0.3f, 0.3f);
    }

    void Start()
    {
        buttonImg = gameObject.GetComponent<Image>();
        text = gameObject.GetComponentInChildren<TMP_Text>();
        
    }
}
