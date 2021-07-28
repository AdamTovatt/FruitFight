using UnityEngine;

public static class DetailColorProvider
{
    public static Color GetColor(DetailColor color)
    {
        switch (color)
        {
            case DetailColor.Red:
                return new Color(1, 0, 0);
            case DetailColor.Green:
                return new Color(0, 1, 0);
            case DetailColor.Blue:
                return new Color(0, 0, 1);
            case DetailColor.Yellow:
                return new Color(1, 1, 0);
            case DetailColor.Purple:
                return new Color(0.9f, 0, 1);
            case DetailColor.Orange:
                return new Color(1, 0.41f, 0);
            default:
                throw new System.Exception("This color is not implemented: " + color);
        }
    }

    public static Color ToColor(this DetailColor color)
    {
        return GetColor(color);
    }
}
