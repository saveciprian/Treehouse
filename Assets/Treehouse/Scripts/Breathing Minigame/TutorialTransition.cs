using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTransition : MonoBehaviour
{
    [Header("Fade Overlay")]
    [SerializeField] private Image fadeOverlay; // Fullscreen black UI Image

    [Header("UI GameObjects")]
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private GameObject nextObject;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float holdBlackDuration = 0.5f;

    public void StartTransition()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        
        SetOverlayAlpha(0f);
        fadeOverlay.gameObject.SetActive(true);

        
        yield return StartCoroutine(FadeOverlayAlpha(0f, 1f));

        
        yield return new WaitForSeconds(holdBlackDuration);

        
        tutorialObject.SetActive(false);
        nextObject.SetActive(true);

        
        yield return StartCoroutine(FadeOverlayAlpha(1f, 0f));

        
        fadeOverlay.gameObject.SetActive(false);
    }

    private IEnumerator FadeOverlayAlpha(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(from, to, t);
            SetOverlayAlpha(alpha);
            yield return null;
        }
        SetOverlayAlpha(to);
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color c = fadeOverlay.color;
        fadeOverlay.color = new Color(c.r, c.g, c.b, alpha);
    }
}