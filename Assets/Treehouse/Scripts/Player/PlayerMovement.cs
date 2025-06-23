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

    //INPUT
    public PlayerInput PlayerControls;
    private InputAction move;
    private InputAction look;
    private InputAction shoot;
    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private bool isShooting;

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
        shoot = PlayerControls.Player.Fire;
        move.Enable();
        look.Enable();

        //clicking and tapping on screen
        shoot.performed += OnShootPerformed;
        shoot.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        look.Enable();

        shoot.performed -= OnShootPerformed;
        shoot.Disable();
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

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Click!");

        /*
            will need to see if tapping on the joysticks is opaque, otherwise need to also block interaction in that case
            how would it work with dynamic joysticks? would be interesting to only allowing tap to select in a specific area
            or, alternatively could just do it so that the player constantly scans for things that overlap the targeting reticle, and on mobile you have an "interact" button that gets enabled... I think that would be the cleanest solution
            this also means that the clicking input should just enable the behaviour, but the scanning would happen in the update loop
        */
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
