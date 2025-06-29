using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTransition : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private GameObject nextObject;
    [SerializeField] private float fadeDuration = 1.5f;

    public void StartTransition()
    {
        StartCoroutine(FadeOutTutorial());
    }

    private IEnumerator FadeOutTutorial()
    {
        Color originalColor = backgroundImage.color;
        float startAlpha = originalColor.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, 0f, t);
            backgroundImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null;
        }

        backgroundImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        tutorialObject.SetActive(false);
        nextObject.SetActive(true);
    }
}