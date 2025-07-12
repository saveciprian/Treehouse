using UnityEngine;
using PixelCrushers.DialogueSystem;

public class UIConversation : MonoBehaviour
{
    [SerializeField] private GameObject mobileUI;
    [SerializeField] private GameObject breathingExercise;

    private void OnConversationStart(Transform actor)
    {
        if (mobileUI != null) mobileUI.SetActive(false);
    }

    private void OnConversationLine(Subtitle subtitle)
    {
        if (subtitle.speakerInfo.IsPlayer)
        {
            if (breathingExercise != null) breathingExercise.SetActive(true);
            if (mobileUI != null) mobileUI.SetActive(false);
        }
    }

    private void OnConversationEnd(Transform actor)
    {
        if (breathingExercise != null) breathingExercise.SetActive(true);

       
        if (mobileUI != null && (breathingExercise == null || !breathingExercise.activeSelf))
        {
            mobileUI.SetActive(true);
        }
        else if (mobileUI != null)
        {
            mobileUI.SetActive(false); 
        }
    }
}