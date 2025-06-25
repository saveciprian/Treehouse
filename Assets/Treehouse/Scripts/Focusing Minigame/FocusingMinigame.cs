using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Make sure to include this for DepthOfField
using UnityEngine.InputSystem;
using System.Collections;

public class FocusingMinigame : MonoBehaviour, IMinigame
{
    public Volume volume;

    DepthOfField dof;
    [SerializeField] private float focusingSpeed = 10f;

    [SerializeField] private float maxFocalLength = 220f;
    private float transitionTime = 300f;
    private float transitionBackTime = 100f;
    private float step = 0.5f;
    private float stepTime = 0.001f;
    private bool enabled = true;

    void OnEnable()
    {
        enabled = true;

        if (volume != null && volume.profile.TryGet<DepthOfField>(out dof))
        {
            dof.mode.value = DepthOfFieldMode.Bokeh; // Or .Gaussian, .Off, etc.
        }
    }

    void OnDisable()
    {
        enabled = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (enabled && dof.focalLength.value < maxFocalLength)
        {
            StopAllCoroutines();
            StartCoroutine(initializeDefocus());
        }
        if (!enabled && dof.focalLength.value > 1)
        {
            StopAllCoroutines();
            StartCoroutine(removeDefocus());
        }

        if (!enabled) return;
        dof.focusDistance.value += InputControls.Instance.moveDirection.y * focusingSpeed * Time.deltaTime;
    }

    void calculateTransitionStepTime(float time)
    {
        float totalSteps = (maxFocalLength - 1f) / step; // 1f is the starting focal length
        stepTime = transitionTime / 1000f / totalSteps; // stepTime in seconds
    }

    IEnumerator initializeDefocus()
    {
        // yield return new WaitForSeconds(stepTime);
        // dof.focalLength.value += step;
        // if (dof.focalLength.value > maxFocalLength) dof.focalLength.value = maxFocalLength;

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
        // yield return new WaitForSeconds(stepTime);
        // dof.focalLength.value -= step;
        // if (dof.focalLength.value < 1)
        // {
        //     dof.focalLength.value = 1;
        //     this.enabled = false;
        // }    
        
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
        this.enabled = false;
    }

    public void Enable()
    {
        calculateTransitionStepTime(transitionTime);
        enabled = true;
    }

    public void Disable()
    {
        calculateTransitionStepTime(transitionBackTime);
        enabled = false;
    }
}
