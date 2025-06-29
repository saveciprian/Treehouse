using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

public class Furball : Character
{
    
    private IMinigame minigame;
    private DialogueSystemTrigger dialogueSystemTrigger;

    void Start()
    {
        minigame = gameObject.GetComponent<IMinigame>();
    }

    public override void Interact()
    {
        base.Interact();

        gameObject.GetComponent<DialogueSystemTrigger>().OnUse();

        //Should start dialogue here, then the dialogue should start the minigame
        // StartMinigame();

    }

    private void StartMinigame()
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
