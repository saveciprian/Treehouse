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
    private int transitionTime = 300;
    private float step = 0.1f;
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
        calculateTransitionStepTime();
    }

    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();

        if (enabled && dof.focalLength.value < maxFocalLength) StartCoroutine(initializeDefocus());
        if (!enabled && dof.focalLength.value > 1) StartCoroutine(removeDefocus());

        dof.focusDistance.value += moveDirection.y * focusingSpeed * Time.deltaTime;
    }

    void calculateTransitionStepTime()
    {
        float totalSteps = (maxFocalLength - 1f) / step; // 1f is the starting focal length
        stepTime = transitionTime / 1000f / totalSteps; // stepTime in seconds
    }

    IEnumerator initializeDefocus()
    {
        yield return new WaitForSeconds(stepTime);
        dof.focalLength.value += step;
        if (dof.focalLength.value > maxFocalLength) dof.focalLength.value = maxFocalLength;
    }
    
    IEnumerator removeDefocus()
    {
        yield return new WaitForSeconds(stepTime);
        dof.focalLength.value -= step;
        if (dof.focalLength.value < 1)
        {
            dof.focalLength.value = 1;
            this.enabled = false;
        }    
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }
}
