using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BreathingDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Rectangle Points")]
    [SerializeField] private RectTransform[] points;
    private Image image;

    [Header("Segment Durations")]
    [SerializeField] private float[] segmentDurations;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] string[] phaseNames;

    [Header("Manual Mode")]
    [SerializeField] private bool isManualMode = false;

    [Header("Manual Threshold Time")]
    [SerializeField] private float timerThreshold;

    [Header("Loop Completion")]
    [SerializeField] private int completedLoops = 0;
    [SerializeField] private bool hasCompletedLoop = false;
    [SerializeField] private int loopsToWin;
    [SerializeField] private float backwardThreshold = 30f; 
    [SerializeField] private float backwardTolerance = 0.5f;

    private RectTransform rectTransform;
    private int currentTargetIndex = 0;
    private bool isDragging = false;
    private float timer = 0f;
    private bool requirePointerReset = false;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private float segmentTime;

    private Vector2 furthestPosition;
    private float backwardTimer = 0f;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.position = points[0].position;
        image = GetComponent<Image>();
        AdvanceSegment();
    }

    private void Update()
    {
        if (!isManualMode && isDragging)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / segmentTime);
            rectTransform.position = Vector2.Lerp(startPoint, endPoint, t);

            if (t >= 1f)
                AdvanceSegment();
        }

        if (isManualMode && isDragging)
        {
            timer += Time.deltaTime;
            UpdateTimerText();

            if (timer >= segmentTime)
                ResetGame();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (requirePointerReset) return;

        if (IsPointerOverCircle(eventData))
        {
            isDragging = true;
            timer = 0f;
            startPoint = rectTransform.position;

            furthestPosition = rectTransform.position;
            backwardTimer = 0f;

            UpdateTimerText();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isManualMode) return;
        if (requirePointerReset) return;

        RectTransform parentRect = rectTransform.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        Vector2 segmentStart = startPoint;
        Vector2 segmentEnd = endPoint;
        Vector2 segmentDir = (segmentEnd - segmentStart).normalized;

        Vector2 fromStartToPoint = localPoint - (Vector2)parentRect.InverseTransformPoint(segmentStart);
        float projectedLength = Vector2.Dot(fromStartToPoint, segmentDir);
        float segmentLength = Vector2.Distance(segmentStart, segmentEnd);

        float clampedLength = Mathf.Clamp(projectedLength, 0, segmentLength);
        Vector2 clampedPoint = (Vector2)parentRect.InverseTransformPoint(segmentStart) + segmentDir * clampedLength;

        rectTransform.localPosition = clampedPoint;

        float currentProgress = clampedLength / segmentLength;

        Vector2 currentWorldPos = rectTransform.position;
        if (CheckBackwardMovement(currentWorldPos))
        {
            ResetGame();
            return;
        }

        float distance = Vector2.Distance(rectTransform.position, endPoint);

        if (distance < 20f)
        {
            if (timer <= segmentTime - timerThreshold)
                ResetGame();
            else
            {
                if (currentTargetIndex == 0)
                    OnLoopCompleted();

                AdvanceSegment();
            }
        }
    }

    private bool CheckBackwardMovement(Vector2 currentWorldPos)
    {
        Vector2 segmentDir = (endPoint - startPoint).normalized;
        float currentDistanceAlongSegment = Vector2.Dot(currentWorldPos - startPoint, segmentDir);
        float furthestDistanceAlongSegment = Vector2.Dot(furthestPosition - startPoint, segmentDir);

        if (currentDistanceAlongSegment > furthestDistanceAlongSegment)
        {
            furthestPosition = currentWorldPos;
            backwardTimer = 0f;
            return false;
        }

        float backwardDistance = furthestDistanceAlongSegment - currentDistanceAlongSegment;

        if (backwardDistance > backwardThreshold)
        {
            backwardTimer += Time.deltaTime;

            if (backwardTimer >= backwardTolerance)
                return true;
        }
        else
            backwardTimer = 0f;

        return false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (timer < segmentTime)
            ResetGame();

        isDragging = false;
        requirePointerReset = false;
    }

    private void ResetGame()
    {
        currentTargetIndex = 0;
        rectTransform.position = points[0].position;
        hasCompletedLoop = false;

        AdvanceSegment();

        furthestPosition = rectTransform.position;
        backwardTimer = 0f;

        requirePointerReset = true;
        isDragging = false;
    }

    private void AdvanceSegment()
    {
        currentTargetIndex = (currentTargetIndex + 1) % points.Length;
        SetNextTarget();
        timer = 0f;

        furthestPosition = rectTransform.position;
        backwardTimer = 0f;

        UpdateTimerText();
    }

    private void OnLoopCompleted()
    {
        completedLoops++;
        hasCompletedLoop = true;

        if (completedLoops >= loopsToWin)
            winText.gameObject.SetActive(true);
    }

    private void SetNextTarget()
    {
        startPoint = rectTransform.position;
        endPoint = points[currentTargetIndex].position;
        segmentTime = segmentDurations[currentTargetIndex];

        int phaseIndex = (currentTargetIndex - 1 + phaseNames.Length) % phaseNames.Length;
        phaseText.text = phaseNames[phaseIndex];
    }

    private void UpdateTimerText()
    {
        float remainingTime = Mathf.Max(segmentTime - timer, 0f);
        timerText.text = "Time Limit: " + remainingTime.ToString("F0") + " s";
    }

    private bool IsPointerOverCircle(PointerEventData eventData)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, eventData.pressEventCamera);
    }
}