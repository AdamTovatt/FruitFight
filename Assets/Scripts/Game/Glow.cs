using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public MeshRenderer ParentRenderer;

    private static Dictionary<Texture, Color> glowColors;

    private void Start()
    {
        if (glowColors == null)
            glowColors = new Dictionary<Texture, Color>();

        ParticleSystem.MainModule mainModule = ParticleSystem.main;
        Texture mainTexutre = ParentRenderer.material.mainTexture;

        Color glowColor = Color.black;
        if (glowColors.ContainsKey(mainTexutre))
        {
            glowColor = glowColors[mainTexutre];
        }
        else
        {
            Texture2D texture2D = ParentRenderer.material.mainTexture as Texture2D;

            Color averageColor = texture2D.GetPixel((int)(texture2D.width * 0.25f), (int)(texture2D.height * 0.25f));
            averageColor += texture2D.GetPixel((int)(texture2D.width * 0.75f), (int)(texture2D.height * 0.25f));
            averageColor += texture2D.GetPixel((int)(texture2D.width * 0.75f), (int)(texture2D.height * 0.75f));
            averageColor += texture2D.GetPixel((int)(texture2D.width * 0.25f), (int)(texture2D.height * 0.75f));
            averageColor = averageColor / 4;

            glowColors.Add(mainTexutre, averageColor);
            glowColor = averageColor;
        }

        mainModule.startColor = glowColor;
    }
}
