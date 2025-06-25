using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class Character : MonoBehaviour, IInteractable
{
    public Material outlineMat;
    private MeshRenderer mr;
    [SerializeField] CinemachineCamera interactCam;


    void Awake()
    {
        
    }

    void Start()
    {
        mr = gameObject.GetComponent<MeshRenderer>();
        outlineMat = mr.materials[1];

        
    }

    void OnEnable()
    {
        InputControls.EscapeKey += StopInteraction; 
    }

    void OnDisable()
    {
        InputControls.EscapeKey -= StopInteraction; 
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
