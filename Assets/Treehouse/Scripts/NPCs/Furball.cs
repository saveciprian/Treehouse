using Unity.VisualScripting;
using UnityEngine;

public class Furball : Character
{
    
    private IMinigame minigame;

    void Start()
    {
        minigame = gameObject.GetComponent<IMinigame>();
    }

    public override void Interact()
    {
        base.Interact();

        StartInteraction();
    }

    private void StartInteraction()
    {
        if (minigame != null)
        {
            ((MonoBehaviour)minigame).enabled = true;
            minigame.Enable();
        }    
    }

    public override void StopInteraction()
    {
        base.StopInteraction();

        if (minigame != null) minigame.Disable();

    }
}
