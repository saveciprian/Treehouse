using UnityEngine;
using UnityEngine.InputSystem;

public class InputControls : MonoBehaviour
{
    public static InputControls Instance { get; private set; }

    public PlayerInput PlayerControls;
    private InputAction move;
    private InputAction look;

    public Vector2 moveDirection { get; private set; }
    public Vector2 lookDirection { get; private set; }

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


    }

    private void OnDisable()
    {
        move.Disable();
        look.Enable();
    }

    void Start()
    {
        
    }


    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        moveDirection.Normalize();

        lookDirection = look.ReadValue<Vector2>();
        // lookDirection.Normalize();
    }
}
