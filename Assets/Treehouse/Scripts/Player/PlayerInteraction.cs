using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{

    private InputAction shoot;
    public PlayerInput PlayerControls;
    private Ray ray;
    private Camera playerCamera;
    private IInteractable _interactableObject;


    private void OnEnable()
    {
        shoot = PlayerControls.Player.Fire;

        //clicking and tapping on screen
        shoot.performed += OnShootPerformed;
        shoot.Enable();
    }

    void OnDisable()
    {
        shoot.performed -= OnShootPerformed;
        shoot.Disable();
    }

    void Awake()
    {
        PlayerControls = new PlayerInput();
    }

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (_interactableObject != null) _interactableObject.Outline();
    }

    void FixedUpdate()
    {
        RaycastHit _hit;

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit))
        {
            _interactableObject = _hit.collider.GetComponent<IInteractable>();
        }
        

    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        if (_interactableObject != null) _interactableObject.Interact();

        /*
            will need to see if tapping on the joysticks is opaque, otherwise need to also block interaction in that case
            how would it work with dynamic joysticks? would be interesting to only allowing tap to select in a specific area
            or, alternatively could just do it so that the player constantly scans for things that overlap the targeting reticle, and on mobile you have an "interact" button that gets enabled... I think that would be the cleanest solution
            this also means that the clicking input should just enable the behaviour, but the scanning would happen in the update loop
        */
    }

}
