using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class Character : MonoBehaviour, IInteractable
{
    public Material outlineMat;
    private MeshRenderer mr;
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

    void Start()
    {
        mr = gameObject.GetComponent<MeshRenderer>();
        outlineMat = mr.materials[1];
    }

    void Update()
    {
        outlineMat.SetFloat("_OutlineThickness", 0f);
    }

    public virtual void Interact()
    {
        //whatever interaction add here
        StartInteraction();

    }

    private void StartInteraction()
    {
        if (!interactCam.IsLive)
        {
            interactCam.Priority = 11;
        }
    }

    public virtual void StopInteraction()
    {
        if (interactCam.IsLive)
        {
            interactCam.Priority = 0;
        }
    }

    void IInteractable.Outline()
    {
        outlineMat.SetFloat("_OutlineThickness", 0.03f);
    }

}
