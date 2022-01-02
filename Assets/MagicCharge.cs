using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCharge : MonoBehaviour
{
    public float Scale = 1f;

    private float timeToFullScale;
    private float startTime;
    private Vector3 startPosition;

    private Transform[] children;

    public void Initialize(float timeToFullScale)
    {
        children = gameObject.GetComponentsInChildren<Transform>();
        startPosition = transform.localPosition;
        startTime = Time.time;
        this.timeToFullScale = timeToFullScale;
    }

    private void Update()
    {
        transform.localPosition = startPosition + GetShakePosition(Time.time, 25, 0.1f);
        Vector3 scale = GetScale();
        foreach (Transform childTransform in children)
        {
            childTransform.localScale = scale * Scale;
        }
    }

    private Vector3 GetScale()
    {
        float time = Time.time - startTime;
        if (time >= timeToFullScale)
            return new Vector3(1, 1, 1);

        float scale = Mathf.Clamp(Mathf.Sin((time * Mathf.PI * 0.5f / timeToFullScale) - Mathf.PI / 2f) + 1, 0, 1);
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
