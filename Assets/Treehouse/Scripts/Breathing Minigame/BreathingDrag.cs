using TMPro;
using UnityEngine;
using System.Collections;

public class BreathingDrag : MonoBehaviour
{
    [Header("Rectangle Points")]
    [SerializeField] private RectTransform[] points;

    [Header("Segment Durations")]
    [SerializeField] private float[] segmentDurations;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private string[] phaseNames;

    [Header("Loop Completion")]
    [SerializeField] private int completedLoops = 0;
    [SerializeField] private int loopsToWin;

    [Header("Transition")]
    [SerializeField] private TutorialTransition tutorialTransition;

	//[SerializeField] private GameObject mobileUI;

    private RectTransform rectTransform;
    private int currentTargetIndex = 0;
    private float timer = 0f;
    private bool isHolding = false;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private float segmentTime;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.position = points[0].position;
        AdvanceSegment();
    }

    private void Update()
    {
        if (isHolding)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / segmentTime);
            rectTransform.position = Vector2.Lerp(startPoint, endPoint, t);

            UpdateTimerText();

            if (t >= 1f)
            {
                if (currentTargetIndex == 0)
                    OnLoopCompleted();

                AdvanceSegment();
            }
        }
    }

    public void OnHoldButtonDown()
    {
        isHolding = true;
    }

    public void OnHoldButtonUp()
    {
        isHolding = false;
        ResetLoop();
    }

    private void AdvanceSegment()
    {
        currentTargetIndex = (currentTargetIndex + 1) % points.Length;
        SetNextTarget();
        timer = 0f;
    }

    private void SetNextTarget()
    {
        startPoint = rectTransform.position;
        endPoint = points[currentTargetIndex].position;
        segmentTime = segmentDurations[currentTargetIndex];

        int phaseIndex = (currentTargetIndex - 1 + phaseNames.Length) % phaseNames.Length;
        phaseText.text = phaseNames[phaseIndex];

        UpdateTimerText();
    }

    private void OnLoopCompleted()
    {
        completedLoops++;
        if (completedLoops >= loopsToWin && tutorialTransition != null)
        {
            Debug.Log("Starting tutorial transition...");
            tutorialTransition.StartTransition();
        }
    }

    private void ResetLoop()
    {
        currentTargetIndex = 0;
        completedLoops = 0;
        rectTransform.position = points[0].position;
        winText.gameObject.SetActive(false);
        AdvanceSegment();
        timer = 0f;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        float remainingTime = Mathf.Max(segmentTime - timer, 0f);
        timerText.text = "Time Left: " + remainingTime.ToString("F0") + " s";
    }
}
