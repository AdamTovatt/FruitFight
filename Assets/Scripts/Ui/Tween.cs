using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween : MonoBehaviour
{
    public bool TweenScale = false;
    public float TweenSpeed = 8f;
    public float StartScale = 0.6f;
    public float EndScale = 1f;
    public bool DestoryOnFinished = false;

    private Vector3 startSize;
    private bool startSizeHasBeenSet = false;
    private float lerpValue = 0;
    private bool tweening = true;

    private void Start()
    {
        StartTween();
    }

    public void ResetValues()
    {
        startSize = Vector3.zero;
        startSizeHasBeenSet = false;
    }

    public void StartTween()
    {
        if (!startSizeHasBeenSet)
        {
            startSize = transform.localScale;
            startSizeHasBeenSet = true;
        }

        tweening = true;
        lerpValue = 0;
        transform.localScale = startSize * StartScale;
    }

    private void Update()
    {
        if (tweening)
        {
            if (lerpValue < 1)
            {
                if (TweenScale)
                    transform.localScale = Vector3.Lerp(startSize * StartScale, startSize * EndScale, lerpValue);

                lerpValue += TweenSpeed * Time.deltaTime;
            }
            else
            {
                if (TweenScale)
                    transform.localScale = startSize * EndScale;

                tweening = false;
                
                if(DestoryOnFinished)
                    Destroy(gameObject);
            }
        }
    }

    public void WasShown()
    {
        StartTween();
    }
}
