using UnityEngine;
using UnityEngine.InputSystem;

public class InputControls : MonoBehaviour
{
    public static InputControls Instance { get; private set; }

    public PlayerInput PlayerControls;
    private InputAction move;
    private InputAction look;
    private InputAction esc;

    public Vector2 moveDirection { get; private set; }
    public Vector2 lookDirection { get; private set; }

    public delegate void EscapePressed();
    public static EscapePressed EscapeKey;


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


    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        moveDirection.Normalize();

        lookDirection = look.ReadValue<Vector2>();
        // lookDirection.Normalize();


    }

}
