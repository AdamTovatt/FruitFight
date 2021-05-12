using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public MeshRenderer ParentRenderer;

    private static Dictionary<Hash128, Color> glowColors;

    private void Start()
    {
        if (glowColors == null)
            glowColors = new Dictionary<Hash128, Color>();

        ParticleSystem.MainModule mainModule = ParticleSystem.main;
        Texture mainTexutre = ParentRenderer.material.mainTexture;
        Hash128 textureHash = mainTexutre.imageContentsHash;

        Color glowColor = Color.black;
        if (glowColors.ContainsKey(textureHash))
        {
            glowColor = glowColors[textureHash];
        }
        else
        {
            Texture2D texture2D = ParentRenderer.material.mainTexture as Texture2D;

            Color averageColor = texture2D.GetPixel((int)(texture2D.width * 0.25f), (int)(texture2D.height * 0.25f));
            averageColor += texture2D.GetPixel((int)(texture2D.width * 0.75f), (int)(texture2D.height * 0.25f));
            averageColor += texture2D.GetPixel((int)(texture2D.width * 0.75f), (int)(texture2D.height * 0.75f));
            averageColor += texture2D.GetPixel((int)(texture2D.width * 0.25f), (int)(texture2D.height * 0.75f));
            averageColor = averageColor / 4;

            glowColors.Add(textureHash, averageColor);
            glowColor = averageColor;
        }

        mainModule.startColor = glowColor;
    }
}
