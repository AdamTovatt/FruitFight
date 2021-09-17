using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTransition : MonoBehaviour
{
    public Image Graphic;

    public delegate void DidTransition();
    public event DidTransition OnDidTransition;

    private RectTransform graphicTransform;

    private bool inTransition = false;
    private float transitionStartTime;
    private float transitionTime;
    private Vector2 startPosition;
    private Vector2 startSize;
    private Vector2 endPosition;
    private Vector2 endSize;

    private void Start()
    {
        graphicTransform = Graphic.GetComponent<RectTransform>();
        Graphic.enabled = false;
    }

    private void Update()
    {
        if(inTransition)
        {
            float transitionValue = Mathf.Clamp((Time.time - transitionStartTime) / transitionTime, 0, 1);

            graphicTransform.localPosition = Vector2.Lerp(startPosition, endPosition, transitionValue);
            graphicTransform.sizeDelta = Vector2.Lerp(startSize, endSize, transitionValue);

            if(transitionValue == 0)
            {
                OnDidTransition?.Invoke();
                OnDidTransition = null;
                inTransition = false;
                Graphic.enabled = false;
            }
        }
    }

    public void SetImage(Sprite sprite)
    {
        Graphic.sprite = sprite;
    }

    public void DoTransition(Vector2 startPosition, Vector2 startSize, Vector2 endPosition, Vector2 endSize, float transitionTime)
    {
        Graphic.enabled = true;
        inTransition = true;
        transitionStartTime = Time.time;
        this.transitionTime = transitionTime;
        this.startSize = startSize;
        this.startPosition = startPosition;
        this.endSize = endSize;
        this.endPosition = endPosition;
        graphicTransform.localPosition = startPosition;
        graphicTransform.sizeDelta = startSize;
    }
}
