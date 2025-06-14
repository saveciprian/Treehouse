using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public PlayerInput PlayerControls;

    private InputAction move;

    private Vector2 moveDirection;
    

    private void Awake()
    {
        PlayerControls = new PlayerInput();
    }

    private void OnEnable()
    {
        move = PlayerControls.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveDirection.x * 30, rb.linearVelocity.y, moveDirection.y * 30);
    }
}
