using Unity.VisualScripting;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class Furball : Character
{
    [SerializeField] CinemachineCamera interactCam;

    private PlayerInput PlayerControls;
    private InputAction esc;

    private void OnEnable()
    {
        esc = PlayerControls.Player.Escape;
        esc.performed += OnEscPerformed;
        esc.Enable();
    }

    private void OnDisable()
    {
        esc.performed -= OnEscPerformed;
        esc.Disable();
    }

    private void OnEscPerformed(InputAction.CallbackContext context)
    {
        StopInteraction();
    }

    void Awake()
    {
        PlayerControls = new PlayerInput();
    }

    public override void Interact()
    {
        base.Interact();

        StartInteraction();
    }

    private void StartInteraction()
    {
        if (!interactCam.IsLive)
        {
            interactCam.Priority = 11;
        }
    }

    private void StopInteraction()
    {
        if (interactCam.IsLive)
        {
            interactCam.Priority = 0;
        }
    }
}
