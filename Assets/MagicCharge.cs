using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCharge : MonoBehaviour
{
    public float Scale = 1f;
    public Light Light;

    private float timeToFullScale;
    private float startTime;
    private Vector3 startPosition;
    private Vector3 currentScale;

    private Vector3 cancelScale;
    private Vector3 cancelPosition;
    private bool cancelled;

    private Transform[] children;

    private float maxLightIntensity;
    private float maxLightRange;

    private void Awake()
    {
        maxLightIntensity = Light.intensity;
        maxLightRange = Light.range;

        Light.intensity = 0;
        Light.range = 0;
    }

    public void Initialize(float timeToFullScale)
    {
        children = gameObject.GetComponentsInChildren<Transform>();
        startPosition = transform.localPosition;
        startTime = Time.time;
        this.timeToFullScale = timeToFullScale;
    }

    private void Update()
    {
        Vector3 shakeOffset = GetShakePosition(Time.time, 25, 0.1f);

        if (!cancelled)
        {
            currentScale = GetScaleVector();
            transform.localPosition = startPosition + shakeOffset;
        }
        else
        {
            transform.position = cancelPosition + shakeOffset;
            currentScale = ClampVector(cancelScale - GetScaleVector(), 0, 1);
        }

        Light.intensity = maxLightIntensity * currentScale.x;
        Light.range = maxLightRange * currentScale.x;

        foreach (Transform childTransform in children)
        {
            childTransform.localScale = currentScale * Scale;
        }

        if (cancelled && Time.time - startTime >= timeToFullScale)
        {
            Destroy(gameObject);
        }
    }

    public void Cancel(bool disappearInstantly)
    {
        if(disappearInstantly)
        {
            Destroy(gameObject);
            return;
        }

        timeToFullScale = timeToFullScale * currentScale.x;
        startTime = Time.time;
        cancelled = true;
        cancelScale = currentScale;
        transform.SetParent(null);
        cancelPosition = transform.position;
    }

    private float GetScale()
    {
        float time = Time.time - startTime;
        if (time >= timeToFullScale)
            return 1;

        return Mathf.Clamp(Mathf.Sin((time * Mathf.PI * 0.5f / timeToFullScale) - Mathf.PI / 2f) + 1, 0, 1);
    }

    private Vector3 ClampVector(Vector3 vector, float min, float max)
    {
        return new Vector3(Mathf.Clamp(vector.x, min, max), Mathf.Clamp(vector.y, min, max), Mathf.Clamp(vector.z, min, max));
    }

    private Vector3 GetScaleVector()
    {
        float scale = GetScale();
        return new Vector3(scale, scale, scale);
    }

    private Vector3 GetShakePosition(float time, float speed, float size)
    {
        float x = (Mathf.Sin(time * speed * 3) + Mathf.Cos(time * speed * 2)) * 0.5f * size;
        float y = (Mathf.Sin(1.5f + time * speed * 2) + Mathf.Cos(2 + time * speed * 3)) * 0.5f * size;
        float z = (Mathf.Sin(-1 + time * speed * 3) + Mathf.Cos(1 + time * speed * 2.5f)) * 0.5f * size;
        return new Vector3(x, y, z);
    }
}
