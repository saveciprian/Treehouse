using PixelCrushers.DialogueSystem;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class Furball : Character
{
    private IMinigame minigame;
    private DialogueSystemTrigger dialogueSystemTrigger;
    // [SerializeField] CinemachineCamera interactCam;

    private PlayerInput PlayerControls;
    private InputAction esc;


    void Start()
    {
        minigame = gameObject.GetComponent<IMinigame>();
    }

    void Awake()
    {
        PlayerControls = new PlayerInput();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //I made it a protected virtual in the character class just in case I'd need to also subscribe to some events here
    }

    public override void Interact()
    {
        base.Interact();

        // StartInteraction();

        // Start Dialogue using DialogueSystemTrigger
        var trigger = GetComponent<DialogueSystemTrigger>();
        if (trigger != null && !DialogueManager.IsConversationActive)
        {
            trigger.OnUse();
        }
    }

    public override void StopInteraction()
    {
        
    }
}