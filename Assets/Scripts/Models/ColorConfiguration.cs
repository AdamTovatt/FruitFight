using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColorConfiguration
{
    public DetailColor Color;
    public Texture2D Texture;

    public static Dictionary<DetailColor, Texture2D> GetLookup(List<ColorConfiguration> colors)
    {
        Dictionary<DetailColor, Texture2D> result = new Dictionary<DetailColor, Texture2D>();
        colors.ForEach(c => result.Add(c.Color, c.Texture));
        return result;
    }
}
