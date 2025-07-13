using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private bool testingMobile = false;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float lookSpeed = 30f;

    [SerializeField] private Joystick movementJoystick;
    [SerializeField] private Joystick lookJoystick;
    [SerializeField] private GameObject mobileUI;

    [SerializeField] private CinemachineCamera playerCamera;
    private Rigidbody rb;

    private float verticalRotation = 0f;
    float minVerticalLookAngle = -70f;
    float maxVerticalLookAngle = 70f;


    private InputControls input;
    private Vector2 moveDir;
    private Vector2 lookDir;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        // playerCamera = gameObject.;

        input = InputControls.Instance;

        if (testingMobile)
        {
            //enable UI
            mobileUI.SetActive(true);
        }

        HideCursor();
        InputControls.ControlSchemeChanged += UpdateControl;
    }

    private void UpdateControl()
    {
        if (input.mode == InputControls.controlMode.Freeroam) HideCursor();
        else ShowCursor();
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (testingMobile)
        {
            moveDir = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
            lookDir = new Vector2(lookJoystick.Horizontal, lookJoystick.Vertical);

        }
        else
        {
            moveDir = input.moveDirection;
            lookDir = input.lookDirection;

        }
        if (moveDir.magnitude > 1) moveDir.Normalize();
        Debug.Log(moveDir);

        if (playerCamera.IsLive)
        {
            transform.Rotate(Vector3.up * lookDir.x * Time.deltaTime * lookSpeed);

            verticalRotation -= lookDir.y * Time.deltaTime * lookSpeed;
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalLookAngle, maxVerticalLookAngle);

            playerCamera.transform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (playerCamera.IsLive)
        { 
            rb.MovePosition(rb.position + transform.forward * movementSpeed * Time.deltaTime * moveDir.y);
            rb.MovePosition(rb.position + transform.right * movementSpeed * Time.deltaTime * moveDir.x);
        }
    }

}
