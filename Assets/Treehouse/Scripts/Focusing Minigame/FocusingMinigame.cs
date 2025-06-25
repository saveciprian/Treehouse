using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Make sure to include this for DepthOfField
using UnityEngine.InputSystem;
using System.Collections;

public class FocusingMinigame : MonoBehaviour, IMinigame
{
    public Volume volume;

    public PlayerInput PlayerControls;
    private InputAction move;
    private Vector2 moveDirection;
    DepthOfField dof;
    [SerializeField] private float focusingSpeed = 10f;

    private float maxFocalLength = 220f;
    private float transitionTime = 300f;
    private float transitionBackTime = 100f;
    private float step = 0.5f;
    private float stepTime = 0.001f;
    private bool enabled = true;

    void Awake()
    {
        PlayerControls = new PlayerInput();
    }

    void OnEnable()
    {
        move = PlayerControls.Player.Move;
        move.Enable();
        enabled = true;

        Debug.Log("Hello there!");
        if (volume != null && volume.profile.TryGet<DepthOfField>(out dof))
        {
            dof.mode.value = DepthOfFieldMode.Bokeh; // Or .Gaussian, .Off, etc.
        }
    }

    void OnDisable()
    {
        move.Disable();
    }

    void Start()
    {
        
    }

    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();

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

        dof.focusDistance.value += moveDirection.y * focusingSpeed * Time.deltaTime;
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
