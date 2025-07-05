using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using PixelCrushers.DialogueSystem;

public class Furball : Character
{
    [SerializeField] private CinemachineCamera interactCam;

    private PlayerInput playerControls;
    private InputAction escAction;

    private void Awake()
    {
        playerControls = new PlayerInput();
    }

    private void OnEnable()
    {
        escAction = playerControls.Player.Escape;
        escAction.performed += OnEscPerformed;
        escAction.Enable();
    }

    private void OnDisable()
    {
        escAction.performed -= OnEscPerformed;
        escAction.Disable();
    }

    private void OnEscPerformed(InputAction.CallbackContext context)
    {
        StopInteraction();
    }

    public override void Interact()
    {
        base.Interact();

        StartInteraction();

        // Start Dialogue using DialogueSystemTrigger
        var trigger = GetComponent<DialogueSystemTrigger>();
        if (trigger != null && !DialogueManager.IsConversationActive)
        {
            trigger.OnUse();
        }
    }

    private void StartInteraction()
    {
        if (interactCam != null && !interactCam.IsLive)
        {
            interactCam.Priority = 11; // Make it active
        }
    }

    private void StopInteraction()
    {
        if (interactCam != null && interactCam.IsLive)
        {
            interactCam.Priority = 0; // Lower priority to deactivate
        }
    }
}