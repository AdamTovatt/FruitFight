using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    /// <summary>
    /// Controls what dimensions are being shaked
    /// </summary>
    public Vector3 ShakeVector;
    public float Magnitude;
    public float Speed;

    public bool UseLimits;
    public Vector3 UpperLimit;
    public Vector3 LowerLimit;

    public int Seed;

    public bool IsShaking { get; private set; }

    private Vector3 lastShakePosition;
    private float timeOffset;

    private void Awake()
    {
        System.Random random = new System.Random(Seed);
        timeOffset = (float)random.NextDouble() * 1000f;
    }

    private void Update()
    {
        if (IsShaking)
        {
            Vector3 originalPosition = transform.localPosition - lastShakePosition;
            lastShakePosition = GetShakePosition(Time.time + timeOffset, Speed, Magnitude);
            
            if (UseLimits)
                lastShakePosition = LimitShake(lastShakePosition);

            transform.localPosition = originalPosition + lastShakePosition;
        }
    }

    public void StartShaking()
    {
        IsShaking = true;
    }

    public void StopShaking()
    {
        IsShaking = false;
        transform.localPosition = transform.localPosition - lastShakePosition;
    }

    private Vector3 LimitShake(Vector3 shakeOffset)
    {
        float x = Mathf.Clamp(shakeOffset.x, LowerLimit.x, UpperLimit.x);
        float y = Mathf.Clamp(shakeOffset.y, LowerLimit.y, UpperLimit.y);
        float z = Mathf.Clamp(shakeOffset.z, LowerLimit.z, UpperLimit.z);
        return new Vector3(x, y, z);
    }

    private Vector3 GetShakePosition(float time, float speed, float size)
    {
        float x = (Mathf.Sin(time * speed * 3) + Mathf.Cos(time * speed * 2)) * 0.5f * size;
        float y = (Mathf.Sin(1.5f + time * speed * 2) + Mathf.Cos(2 + time * speed * 3)) * 0.5f * size;
        float z = (Mathf.Sin(-1 + time * speed * 3) + Mathf.Cos(1 + time * speed * 2.5f)) * 0.5f * size;
        return new Vector3(x * ShakeVector.x, y * ShakeVector.y, z * ShakeVector.z);
    }
}
