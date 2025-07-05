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
        if (subtitle.speakerInfo.IsPlayer && breathingExercise != null)
        {
            breathingExercise.SetActive(true);
        }
    }

    private void OnConversationEnd(Transform actor)
    {
        if (mobileUI != null) mobileUI.SetActive(true);
        if (breathingExercise != null) breathingExercise.SetActive(true);
    }
}