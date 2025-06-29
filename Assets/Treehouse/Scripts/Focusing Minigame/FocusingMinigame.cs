using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Make sure to include this for DepthOfField
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

public class FocusingMinigame : MonoBehaviour, IMinigame
{
    public Volume volume;

    DepthOfField dof;
    [SerializeField] private GameObject FocusingUI;
    [SerializeField] private float focusingSpeed = 10f;
    [SerializeField] private float maxFocalLength = 100f;
    private float transitionTime = 300f;
    private float transitionBackTime = 100f;
    private float step = 0.5f;
    private float stepTime = 0.001f;
    private bool minigameEnabled = true;
    private float focusDistance;

    [SerializeField] private float minFocus = 0f;
    [SerializeField] private float maxFocus = 100f;
    private Camera playCam;
    [SerializeField] private float hitFuzz = 1f;

    public List<GameObject> solutionObjects;
    // public List<Image> solutionIndicators

    void OnEnable()
    {
        
        minigameEnabled = true;
        FocusingUI.SetActive(true);

        if (volume != null && volume.profile.TryGet<DepthOfField>(out dof))
        {
            dof.mode.value = DepthOfFieldMode.Bokeh; // Or .Gaussian, .Off, etc.
        }

        InputControls.pointerDown += SelectObject;
    }

    void OnDisable()
    {
        FocusingUI.SetActive(false);
        minigameEnabled = false;
        InputControls.pointerDown -= SelectObject;
    }

    public void SelectObject()
    {
        playCam = Camera.main;
        Vector3 screenPos = InputControls.Instance.getPointerPos();
        Ray ray = playCam.ScreenPointToRay(screenPos);
        RaycastHit _hit;

        if (Physics.Raycast(ray, out _hit))
        {
            // Debug.Log("Hit: " + _hit.collider.name);

            if (focusDistance - hitFuzz < _hit.distance && _hit.distance < focusDistance + hitFuzz) Debug.Log("object hit: " + _hit.collider.name);
        }
    }

    void Start()
    {
     
    }

    void Update()
    {
        if (minigameEnabled && dof.focalLength.value < maxFocalLength)
        {
            StopAllCoroutines();
            StartCoroutine(initializeDefocus());
        }
        if (!minigameEnabled && dof.focalLength.value > 1)
        {
            StopAllCoroutines();
            StartCoroutine(removeDefocus());
        }

        // dof.focusDistance.value += InputControls.Instance.moveDirection.y * focusingSpeed * Time.deltaTime;
    }

    public void ModifyFocusDistance(float dt)
    {
        if (!minigameEnabled) return;
        focusDistance -= dt * focusingSpeed * Time.deltaTime;
        focusDistance = clamp(focusDistance, minFocus, maxFocus);

        dof.focusDistance.value = focusDistance;
    }

    private float clamp(float value, float min, float max)
    {
        if (value < min) return min;
        else if (value > max) return max;
        else return value;
    }

    void calculateTransitionStepTime(float time)
    {
        float totalSteps = (maxFocalLength - 1f) / step; // 1f is the starting focal length
        stepTime = transitionTime / 1000f / totalSteps; // stepTime in seconds
    }

    IEnumerator initializeDefocus()
    {
        float start = dof.focalLength.value;
        float end = maxFocalLength;
        float duration = transitionTime / 1000f; // ms to seconds
        float elapsed = 0f;

        while (elapsed < duration)
        {
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            dof.focalLength.value = Mathf.Lerp(start, end, t);
        }
        dof.focalLength.value = end;
    }

    IEnumerator removeDefocus()
    {   
        float start = dof.focalLength.value;
        float end = 1f;
        float duration = transitionBackTime / 1000f; // ms to seconds
        float elapsed = 0f;

        while (elapsed < duration)
        {
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            dof.focalLength.value = Mathf.Lerp(start, end, t);
        }
        dof.focalLength.value = end;
        this.minigameEnabled = false;
    }

    public void Enable()
    {
        calculateTransitionStepTime(transitionTime);
        minigameEnabled = true;
        focusDistance = dof.focusDistance.value;
    }

    public void Disable()
    {
        calculateTransitionStepTime(transitionBackTime);
        minigameEnabled = false;
    }
}
