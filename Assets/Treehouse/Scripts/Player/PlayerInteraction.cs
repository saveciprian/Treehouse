using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerInteraction : MonoBehaviour
{

    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private float interactionDistance = 10f;
    private IInteractable _interactableObject;
    [SerializeField] private GameObject interactButton;

    void Awake()
    {
        
    }

    void Start()
    {

    }

    void Update()
    {
        //Need to replace all playerCamera.IsLive with a different system. Maybe observer on the actual virtual camera?
        if (_interactableObject != null && playerCamera.IsLive) _interactableObject.Outline();
    }

    void FixedUpdate()
    {
        RaycastHit _hit;

        interactButton.SetActive((_interactableObject != null) ? true : false);

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, interactionDistance) && playerCamera.IsLive)
        {
            _interactableObject = _hit.collider.GetComponent<IInteractable>();
            
        }
        else
        {
            _interactableObject = null;
            
        }


    }

    public void Interact()
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
