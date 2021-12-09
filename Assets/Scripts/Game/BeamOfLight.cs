using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamOfLight : MonoBehaviour
{
    public MeshRenderer Inner;
    public MeshRenderer Outer;

    public Color MinColor;
    public Color MaxColor;

    public Light Light;

    public float MaxLightIntensity;
    public float AnimationTime;
    public float PeakMultiplier = 1.1f;

    public delegate void ReachedPeakHandler();
    public ReachedPeakHandler OnReachedPeak;

    private float lerpValue;
    private bool reachedPeak = false;

    public void Start()
    {
        Light.intensity = 0;
        Inner.material.color = MinColor;
        Outer.material.color = MinColor;
    }

    public void Update()
    {
        float yValue = Mathf.Clamp(Mathf.Sin((lerpValue * Mathf.PI) / AnimationTime) * PeakMultiplier, -1, 1);

        Light.intensity = MaxLightIntensity * yValue;
        Color color = Color.Lerp(MinColor, MaxColor, yValue);
        Inner.material.color = color;
        Outer.material.color = color;

        if (!reachedPeak && yValue == 1)
        {
            OnReachedPeak?.Invoke();
            OnReachedPeak = null;
            reachedPeak = true;
        }

        lerpValue += Time.deltaTime;
    }
}
