using System.Collections.Generic;
using UnityEngine;

public class DetailColorController : MonoBehaviour
{
    public List<Renderer> DetailRenderers;
    public DetailColor Color;
    public List<ColorConfiguration> Colors;
    public bool StartWithEmission = false;

    public float CurrentEmission { get; private set; }

    private Dictionary<DetailColor, Texture2D> detailColors;

    private void Awake()
    {
        detailColors = ColorConfiguration.GetLookup(Colors);
    }

    private void Start()
    {
        if (StartWithEmission)
            SetEmission(1);
        else
            SetEmission(0);

        SetTextureFromColor();
    }

    public void SetTextureFromColor()
    {
        foreach (Renderer _renderer in DetailRenderers)
            _renderer.material.mainTexture = detailColors[Color];
    }

    public void SetEmission(float emission)
    {
        CurrentEmission = emission;

        foreach (Renderer _renderer in DetailRenderers)
        {
            _renderer.material.EnableKeyword("_EMISSION");
            Color finalColor = Color.ToColor() * Mathf.LinearToGammaSpace(emission);
            _renderer.material.SetColor("_EmissionColor", finalColor);
        }
    }
}
