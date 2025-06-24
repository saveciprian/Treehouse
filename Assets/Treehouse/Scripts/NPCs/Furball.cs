using UnityEngine;

public class Furball : MonoBehaviour, IInteractable
{
    public Material outlineMat;

    private MeshRenderer mr;

    void Start()
    {
        mr = gameObject.GetComponent<MeshRenderer>();
        outlineMat = mr.materials[1];
    }

    void Update()
    {
        outlineMat.SetFloat("_OutlineThickness", 0f);
    }

    void IInteractable.Interact()
    {
        //whatever interaction add here


    }

    void IInteractable.Outline()
    {
        outlineMat.SetFloat("_OutlineThickness", 0.03f);
    }

}
