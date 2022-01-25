using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AnimatedCountText : MonoBehaviour
{
    public delegate void FinishedAnimatingHandler();
    public event FinishedAnimatingHandler OnFinishedAnimating;
    
    private TextMeshProUGUI text;
    private float animationTime;
    private int currentCount;
    private int deltaCount;
    private int minCount;
    private int maxCount;
    private int startCount;
    private float startTime;
    private bool animating;

    private void Awake()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if(animating)
        {
            float passedTime = Time.time - startTime;

            if(passedTime < animationTime)
            {
                currentCount = Mathf.Clamp(Mathf.RoundToInt(startCount + (deltaCount * Mathf.Sin(passedTime * (Mathf.PI / (2 * animationTime))))), minCount, maxCount);
                text.text = currentCount.ToString();
            }
            else
            {
                animating = false;
                text.text = (startCount + deltaCount).ToString();
                OnFinishedAnimating?.Invoke();
                OnFinishedAnimating = null;
            }
        }
    }

    public void StartAnimation(int startCount, int endCount, float animationTime = 1.0f)
    {
        minCount = Mathf.Min(startCount, endCount);
        maxCount = Mathf.Max(startCount, endCount);
        this.startCount = startCount;
        this.animationTime = animationTime;
        deltaCount = endCount - startCount;
        animating = true;
        startTime = Time.time;
        currentCount = startCount;
        text.text = currentCount.ToString();
    }
}
