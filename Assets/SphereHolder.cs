using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereHolder : MonoBehaviour
{
    public DetailColor Color;
    public ParticleSystem Smoke;
    public MeshRenderer SphereHolderRenderer;
    public float SmokeAlpha = 0.2f;
    public List<ColorConfiguration> Colors;

    private StateSwitcher switcher;
    private Dictionary<DetailColor, Texture2D> detailColors;

    private void Awake()
    {
        detailColors = ColorConfiguration.GetLookup(Colors);
    }

    private void Start()
    {
        switcher = gameObject.GetComponent<StateSwitcher>();
        switcher.OnActivated += Activated;
        switcher.OnDeactivated += Deactvated;

        Color color = Color.ToColor();
        ParticleSystem.MainModule smokeMain = Smoke.main;
        smokeMain.startColor = new ParticleSystem.MinMaxGradient(new Color(color.r, color.g, color.b, SmokeAlpha), new Color(color.r, color.g, color.b, SmokeAlpha * 0.8f));

        SphereHolderRenderer.material.mainTexture = detailColors[Color];
        SetEmission(0);
    }

    private void Activated()
    {
        Debug.Log("smoke activeated");
        Smoke.Play();
        SetEmission(1);
    }

    private void Deactvated()
    {
        Debug.Log("Smoke deactivated");
        Smoke.Stop();
        SetEmission(0);
    }

    private void OnDestroy()
    {
        switcher.OnActivated -= Activated;
        switcher.OnDeactivated -= Deactvated;
    }

    private void SetEmission(float emission)
    {
        SphereHolderRenderer.material.EnableKeyword("_EMISSION");

        Color finalColor = Color.ToColor() * Mathf.LinearToGammaSpace(emission);

        SphereHolderRenderer.material.SetColor("_EmissionColor", finalColor);
    }
}
