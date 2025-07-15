using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TutorialTransition : MonoBehaviour
{
   
    [Header("Fade Overlay")]
    [SerializeField] private CanvasGroup fadeOverlay;

    [Header("UI GameObjects")]
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private GameObject nextObject;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float holdBlackDuration = 1f;

    public void StartTransition()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        fadeOverlay.gameObject.SetActive(true);

        yield return StartCoroutine(FadeOverlayAlpha(0f, 0.5f));

        yield return new WaitForSeconds(holdBlackDuration);

        tutorialObject.SetActive(false);
        nextObject.SetActive(true);

        yield return StartCoroutine(FadeOverlayAlpha(0.5f, 0f));

        fadeOverlay.gameObject.SetActive(false);
    }

    private IEnumerator FadeOverlayAlpha(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadeOverlay.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        fadeOverlay.alpha = to;
    }
}