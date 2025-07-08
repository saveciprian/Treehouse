using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 

public class HoldToLoadLevel : MonoBehaviour
{
    [Header("Radial Timers")]
    [SerializeField] private float indicatorTimer = 0.2f;
    [SerializeField] private float maxIndicatorTimer = 0.2f;

    [Header("UI Elements")]
    [SerializeField] private Image radicalIndicatorUI;

    [Header("Unity Event")]
    [SerializeField] private UnityEvent myEvent = null;

    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    private bool isHolding = false;

    private void Update()
    {
        if (isHolding)
        {
            indicatorTimer -= Time.deltaTime;
            radicalIndicatorUI.enabled = true;
            radicalIndicatorUI.fillAmount = 1f - (indicatorTimer / maxIndicatorTimer);

            if (indicatorTimer <= 0)
            {
                indicatorTimer = maxIndicatorTimer;
                radicalIndicatorUI.fillAmount = 0f;
                radicalIndicatorUI.enabled = false;
                isHolding = false;
                
                
                myEvent?.Invoke();

                // Load the scene
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    SceneManager.LoadScene(sceneToLoad);
                }
                else
                {
                    Debug.LogWarning("No scene name set in HoldToLoadLevel script.");
                }
            }
        }
        else
        {
            if (radicalIndicatorUI.enabled)
            {
                indicatorTimer += Time.deltaTime;
                radicalIndicatorUI.fillAmount = 1f - (indicatorTimer / maxIndicatorTimer);

                if (indicatorTimer >= maxIndicatorTimer)
                {
                    indicatorTimer = maxIndicatorTimer;
                    radicalIndicatorUI.fillAmount = 0f;
                    radicalIndicatorUI.enabled = false;
                }
            }
        }
    }

    public void OnHoldButtonDown()
    {
        isHolding = true;
    }

    public void OnHoldButtonUp()
    {
        isHolding = false;
    }
}
