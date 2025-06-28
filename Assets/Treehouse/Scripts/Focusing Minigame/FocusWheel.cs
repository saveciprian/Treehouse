using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using System;
using System.Drawing;

public class FocusWheel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool rotatingDial = false;
    [SerializeField] FocusingMinigame minigame;
    private Vector2 previousPos;
    private Vector2 currentPos;

    private Vector2 firstAngle;
    private Vector2 secondAngle;
    private RectTransform rect;
    private float deltaRotation;
    [SerializeField] private float rotationModifier = 10f;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        // if (rotatingDial) Debug.Log(InputControls.Instance.getPointerPos());


        if (rotatingDial)
        {
            if (previousPos == Vector2.zero)
            {
                previousPos = currentPos;
                return;
            }

            currentPos = InputControls.Instance.getPointerPos();

            Vector2 center = rect.position;
            float prevAngle = Mathf.Atan2(previousPos.y - center.y, previousPos.x - center.x) * Mathf.Rad2Deg;
            float currAngle = Mathf.Atan2(currentPos.y - center.y, currentPos.x - center.x) * Mathf.Rad2Deg;

            float deltaAngle = Mathf.DeltaAngle(prevAngle, currAngle);

            rect.eulerAngles += new Vector3(0, 0, deltaAngle);
            minigame.ModifyFocusDistance(deltaAngle * rotationModifier);
            
            previousPos = currentPos;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rotatingDial = true;
        currentPos = InputControls.Instance.getPointerPos();

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rotatingDial = false;
        previousPos = Vector2.zero;
        currentPos = Vector2.zero;
    }

    
}
