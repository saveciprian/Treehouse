using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 30f;
    [SerializeField] private float lookSpeed = 30f;
    private Camera playerCamera;
    private Rigidbody rb;
    public PlayerInput PlayerControls;
    private InputAction move;
    private InputAction look;
    private Vector2 moveDirection;
    private Vector2 lookDirection;
    

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
    }

    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        moveDirection.Normalize();
        lookDirection = look.ReadValue<Vector2>();

        Debug.Log("Look Direction: " + lookDirection);
        transform.Rotate(Vector3.up * lookDirection.x * Time.deltaTime * lookSpeed);
        playerCamera.transform.Rotate(Vector3.left * lookDirection.y * Time.deltaTime * lookSpeed);

    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * movementSpeed * Time.deltaTime * moveDirection.y);
        rb.MovePosition(rb.position + transform.right * movementSpeed * Time.deltaTime * moveDirection.x);
    }
}
