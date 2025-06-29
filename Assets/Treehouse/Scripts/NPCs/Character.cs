using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class Character : MonoBehaviour, IInteractable
{
    public Material outlineMat;
    private MeshRenderer mr;
    [SerializeField] CinemachineCamera interactCam;

    [SerializeField] CinemachineCamera[] interactCameras;
    private int activeIndex = 0;


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

    public virtual void Interact() //Player calls this function on interact
    {
        StartInteraction();
    }

    private void StartInteraction()
    {
        // if (!interactCam.IsLive)
        // {
        //     interactCam.Priority = 11;
        // }

        setCameraAsActive(0);
        InputControls.Instance.ControlToMinigame();
    }

    public virtual void StopInteraction()
    {
        if (interactCam.IsLive)
        {
            interactCam.Priority = 0;
        }

        InputControls.Instance.ControlToFreeroam();
    }

    private void setCameraAsActive(int index)
    {
        interactCameras[index].Priority = 11;
    }

    private void unsetCameraAsActive(int index)
    {
        interactCameras[index].Priority = 0;
    }

    public void setNextCamera()
    {
        unsetCameraAsActive(activeIndex);
        if (!(activeIndex + 1 >= interactCameras.Length)) activeIndex++;
        setCameraAsActive(activeIndex);
    }

    public void resetToPlayerCam()
    {
        foreach (var camera in interactCameras)
        {
            camera.Priority = 0;
        }

        InputControls.Instance.ControlToFreeroam();
    }

    void IInteractable.Outline()
    {
        outlineMat.SetFloat("_OutlineThickness", 0.03f);
    }

}
