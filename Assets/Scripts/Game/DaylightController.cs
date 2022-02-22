using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class DaylightController : MonoBehaviour
{
    public Light LightSource;
    public LensFlareComponentSRP LensFlare;

    [Range(0.0f, 24.0f)]
    public float TimeOfDay = 15;

    public float StartRotation = 90f;
    public float MaxHeight = 0.5f;
    public float Elevation;
    public float Azimuth;

    public Color CurrentColor;

    public ColorOnTimeOfDay[] Colors;

    public static DaylightController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(float startRotation, float timeOfDay)
    {
        StartRotation = startRotation;
        TimeOfDay = timeOfDay;

        CalculateValuesFromTimeOfDay();
        ApplyAngles();
    }

    private int Modulus(int x, int m)
    {
        return (x % m + m) % m;
    }

    private Color GetColorFromTimeOfDay(float timeOfDay)
    {
        List<ColorOnTimeOfDay> orderedTimes = Colors.OrderBy(x => x.TimeOfDay).ToList();

        ColorOnTimeOfDay lowerColor = Colors.OrderBy(x => Mathf.Abs(x.TimeOfDay - timeOfDay)).First(); //pick a color that is the closest to our given position (i)
        ColorOnTimeOfDay upperColor;

        if (timeOfDay - lowerColor.TimeOfDay >= 0) //the color we picked was lower than our current our so we keep it as "lowerColor"
        {
            upperColor = orderedTimes[Modulus((orderedTimes.IndexOf(orderedTimes.Where(x => x.TimeOfDay == lowerColor.TimeOfDay).First()) + 1), orderedTimes.Count)]; //set the upper color to the next color in the array
        }
        else //the color we picked was actually higher than our current color se we want to make it the upper color
        {
            upperColor = lowerColor;
            lowerColor = orderedTimes[Modulus((orderedTimes.IndexOf(orderedTimes.Where(x => x.TimeOfDay == lowerColor.TimeOfDay).First()) - 1), orderedTimes.Count)];
        }

        float lerpValue = ((timeOfDay - lowerColor.TimeOfDay) / (upperColor.TimeOfDay - lowerColor.TimeOfDay));
        
        Color result = Color.Lerp(lowerColor.Color, upperColor.Color, lerpValue);
        result.a = 1;
        return result;
    }

    private void CalculateValuesFromTimeOfDay()
    {
        Elevation = Mathf.Sin(TimeOfDay * (Mathf.PI / 12f) - (Mathf.PI / 2f)) * (MaxHeight * 90f);
        Azimuth = StartRotation + ((TimeOfDay / 24f) * 360f);

        RenderSettings.ambientIntensity = 1 + Mathf.Sin(TimeOfDay * (Mathf.PI / 12f) - (Mathf.PI / 2f)) * 0.2f;

        CurrentColor = GetColorFromTimeOfDay(TimeOfDay);

        if(LensFlare != null)
        {
            foreach(LensFlareDataElementSRP element in LensFlare.lensFlareData.elements)
            {
                element.tint = CurrentColor;
            }
        }
    }

    private void ApplyAngles()
    {
        transform.eulerAngles = new Vector3(Elevation, Azimuth, 0);
        LightSource.color = CurrentColor;
    }
}

[Serializable]
public class ColorOnTimeOfDay
{
    public float TimeOfDay;
    public Color Color;

    public override string ToString()
    {
        return string.Format("Time: {0} Color: ({0}, {1}, {2})", TimeOfDay, Color.r, Color.g, Color.b);
    }
}
