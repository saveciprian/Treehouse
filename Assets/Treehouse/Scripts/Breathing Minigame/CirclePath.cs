using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CirclePath : MonoBehaviour
{
    [Header("Circular Path")]
    [SerializeField] private float radius = 100f;
    [SerializeField] private Vector2 centerPosition;

    [Header("Segment Durations")]
    [SerializeField] private float[] segmentDurations;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private string[] phaseNames = new string[] { "Inhale", "Hold", "Exhale", "Hold" };

    [Header("Loop Completion")]
    [SerializeField] private int completedLoops = 0;
    [SerializeField] private int loopsToWin;

    [Header("Transition")]
    [SerializeField] private TutorialTransition tutorialTransition;

    [Header("Follower")]
    [SerializeField] private GameObject followerObject;

	[Header("UI Switching")]
	[SerializeField] private GameObject circleBreathingUI;
	[SerializeField] private GameObject mobileUI;

	[Header("Scene Management")]
	[SerializeField] private string sceneToLoad;
	

    private RectTransform rectTransform;
    private bool isHolding = false;
    private float segmentTimer = 0f;
    private float currentAngle = 0f;
    private int currentSegment = 0;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (centerPosition == Vector2.zero)
            centerPosition = rectTransform.anchoredPosition;

        currentAngle = 0f;
        SetPhase(currentSegment);
    }

    private void Update()
    {
        if (isHolding)
        {
            segmentTimer += Time.deltaTime;

            float segmentDuration = segmentDurations[currentSegment];
            float t = Mathf.Clamp01(segmentTimer / segmentDuration);
            float segmentAngle = Mathf.PI * 2f / segmentDurations.Length;
            float startAngle = currentSegment * segmentAngle;
            float targetAngle = startAngle + segmentAngle;
            float angle = Mathf.Lerp(startAngle, targetAngle, t);

            Vector2 pos = new Vector2(
                centerPosition.x + Mathf.Cos(angle) * radius,
                centerPosition.y + Mathf.Sin(angle) * radius
            );

            rectTransform.anchoredPosition = pos;

            if (followerObject != null)
            {
                RectTransform followerRT = followerObject.GetComponent<RectTransform>();
                if (followerRT != null)
                {
                    followerRT.anchoredPosition = pos;
                }
                else
                {
                    followerObject.transform.position = rectTransform.TransformPoint(pos);
                }
            }

            UpdateTimerText(segmentDuration - segmentTimer);

            if (t >= 1f)
            {
                segmentTimer = 0f;
                currentSegment = (currentSegment + 1) % segmentDurations.Length;
                SetPhase(currentSegment);

                if (currentSegment == 0)
				{
    				OnLoopCompleted();
				}
            }
        }
    }

    public void OnHoldButtonDown() => isHolding = true;
    public void OnHoldButtonUp()
    {
        isHolding = false;
        ResetLoop();
    }

    private void SetPhase(int index)
    {
        if (phaseText != null && phaseNames.Length > 0)
            phaseText.text = phaseNames[index % phaseNames.Length];
    }

    private void ResetLoop()
    {
        isHolding = false;
        segmentTimer = 0f;
        currentAngle = 0f;
        currentSegment = 0;
        completedLoops = 0;

        Vector2 startPos = centerPosition + new Vector2(radius, 0);
        rectTransform.anchoredPosition = startPos;

        if (followerObject != null)
        {
            RectTransform followerRT = followerObject.GetComponent<RectTransform>();
            if (followerRT != null)
                followerRT.anchoredPosition = startPos;
            else
                followerObject.transform.position = rectTransform.TransformPoint(startPos);
        }

        winText.gameObject.SetActive(false);
        SetPhase(currentSegment);
        UpdateTimerText(segmentDurations[0]);
    }

	private void OnLoopCompleted()
	{
    	completedLoops++;

    	if (completedLoops >= loopsToWin)
    	{
        	if (tutorialTransition != null)
            tutorialTransition.StartTransition();

        	if (circleBreathingUI != null)
            circleBreathingUI.SetActive(false);

        	if (mobileUI != null)
            mobileUI.SetActive(true);

        	if (!string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
    	}
	}



    private void UpdateTimerText(float remainingTime)
    {
        timerText.text = "Time Left: " + Mathf.Max(remainingTime, 0f).ToString("F0") + " s";
    }

    private void OnDrawGizmos()
    {
        if (segmentDurations == null || segmentDurations.Length == 0) return;

        Vector3 worldCenter = transform.TransformPoint(centerPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(worldCenter, 5f);

        Gizmos.color = Color.cyan;
        int segments = 100;
        Vector3 prevPoint = worldCenter + (Quaternion.Euler(0, 0, 0) * Vector3.right * radius);
        for (int i = 1; i <= segments; i++)
        {
            float angle = 360f * i / segments;
            Vector3 nextPoint = worldCenter + (Quaternion.Euler(0, 0, angle) * Vector3.right * radius);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < segmentDurations.Length; i++)
        {
            float angle = 360f * i / segmentDurations.Length;
            Vector3 marker = worldCenter + (Quaternion.Euler(0, 0, angle) * Vector3.right * radius);
            Gizmos.DrawSphere(marker, 4f);
        }
    }

}
