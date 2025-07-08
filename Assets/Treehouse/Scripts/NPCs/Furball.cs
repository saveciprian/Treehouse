using PixelCrushers.DialogueSystem;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class Furball : Character
{
    private IMinigame minigame;
    private DialogueSystemTrigger dialogueSystemTrigger;
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

    void Start()
    {
        minigame = gameObject.GetComponent<IMinigame>();
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

    private void StartMinigame()
    {
        if (!interactCam.IsLive)
        {
            interactCam.Priority = 11;
        }
    }

    public override void StopInteraction()
    {
        if (interactCam.IsLive)
        {
            interactCam.Priority = 0;
        }
    }
}