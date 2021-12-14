using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public float FlickerSpeed = 1f;
    public float AmplitudeMultiplier = 1f;
    public Light Light;
    public int Waves = 3;

    private LightFlickerWave[] waves;

    private void Start()
    {
        waves = new LightFlickerWave[Waves];

        for (int i = 0; i < Waves; i++)
        {
            waves[i] = new LightFlickerWave()
            {
                Offset = Random.Range(0f, 2f) * Mathf.PI,
                Amplitude = Random.Range(0.1f * AmplitudeMultiplier, 0.3f * AmplitudeMultiplier),
                Width = Random.Range(0.2f, 2f)
            };
        }
    }

    private void Update()
    {
        float x = Time.time;
        float intensity = 0;
        foreach(LightFlickerWave wave in waves)
        {
            intensity += Mathf.Sin((x * FlickerSpeed) * wave.Width + wave.Offset) * wave.Amplitude;
        }

        intensity /= Waves;
        intensity += 1;
        Light.intensity = intensity;
    }
}

public class LightFlickerWave
{
    public float Offset { get; set; }
    public float Amplitude { get; set; }
    public float Width { get; set; }
}
