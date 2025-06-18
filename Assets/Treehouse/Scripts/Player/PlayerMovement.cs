using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool testingMobile = false;
    [SerializeField] private float movementSpeed = 30f;
    [SerializeField] private float lookSpeed = 30f;

    [SerializeField] private Joystick movementJoystick;
    [SerializeField] private Joystick lookJoystick;
    [SerializeField] private GameObject mobileUI;

    private Camera playerCamera;
    private Rigidbody rb;
    public PlayerInput PlayerControls;
    private InputAction move;
    private InputAction look;
    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private float verticalRotation = 0f;
    float minVerticalLookAngle = -70f;
    float maxVerticalLookAngle = 70f;
    

    private void Awake()
    {
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

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (testingMobile)
        {
            //enable UI
            mobileUI.SetActive(true);
        }
    }

    private void Update()
    {
        if (testingMobile)
        {
            moveDirection = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
            lookDirection = new Vector2(lookJoystick.Horizontal, lookJoystick.Vertical);

        }
        else
        {
            moveDirection = move.ReadValue<Vector2>();
            lookDirection = look.ReadValue<Vector2>();

        }
        moveDirection.Normalize();

        transform.Rotate(Vector3.up * lookDirection.x * Time.deltaTime * lookSpeed);

        

        verticalRotation -= lookDirection.y * Time.deltaTime * lookSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalLookAngle, maxVerticalLookAngle);

        playerCamera.transform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * movementSpeed * Time.deltaTime * moveDirection.y);
        rb.MovePosition(rb.position + transform.right * movementSpeed * Time.deltaTime * moveDirection.x);
    }
}
