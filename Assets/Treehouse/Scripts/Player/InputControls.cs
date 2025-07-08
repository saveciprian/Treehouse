using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputControls : MonoBehaviour
{
    public static InputControls Instance { get; private set; }

    public PlayerInput PlayerControls;
    private InputAction move;
    private InputAction look;
    private InputAction esc;
    [SerializeField] private bool testingMobile = false;

    Vector2 mousePos;
    Vector2 touchPos;

    public Vector2 moveDirection { get; private set; }
    public Vector2 lookDirection { get; private set; }
    public enum controlMode
    {
        Freeroam,
        Minigame
    }

    public controlMode mode = controlMode.Freeroam;

    public delegate void ControlModeChanged();
    public static ControlModeChanged ControlSchemeChanged;
    public delegate void EscapePressed();
    public static EscapePressed EscapeKey;

    public delegate void PointerDown();
    public static PointerDown pointerDown;
    public delegate void PointerUp();
    public static PointerUp pointerUp;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        PlayerControls = new PlayerInput();
    }

    private void OnEnable()
    {
        move = PlayerControls.Player.Move;
        look = PlayerControls.Player.Look;

        move.Enable();
        look.Enable();

        esc = PlayerControls.Player.Escape;
        esc.performed += OnEscPerformed;
        esc.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        look.Enable();

        esc.performed -= OnEscPerformed;
        esc.Disable();
    }

    private void OnEscPerformed(InputAction.CallbackContext context)
    {
        EscapeKey?.Invoke();
    }

    public void ControlToFreeroam()
    {
        Debug.LogWarning("Control mode changed to Freeroam");
        mode = controlMode.Freeroam;
        ControlSchemeChanged?.Invoke();
    }

    public void ControlToMinigame()
    {
        Debug.LogWarning("Control mode changed to Minigame");
        mode = controlMode.Minigame;
        ControlSchemeChanged?.Invoke();
    }

    void Start()
    {
        ControlToFreeroam();
    }


    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        moveDirection.Normalize();

        lookDirection = look.ReadValue<Vector2>();
        // lookDirection.Normalize();

        if (testingMobile)
        {
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                pointerDown?.Invoke();

                var touch = Touchscreen.current.touches[0];

                if (touch.press.wasPressedThisFrame) pointerDown?.Invoke();
                if (touch.press.wasReleasedThisFrame) pointerUp?.Invoke();

                touchPos = Touchscreen.current.touches[0].position.ReadValue();
            }
        }
        else
        {
            if(Mouse.current.leftButton.wasPressedThisFrame) pointerDown?.Invoke();
            if(Mouse.current.leftButton.wasReleasedThisFrame) pointerUp?.Invoke();

            mousePos = Mouse.current.position.ReadValue();
            // Debug.Log(mousePos);
        }
    }

    public Vector2 getPointerPos()
    {
        return testingMobile ? touchPos : mousePos;
    }
}
