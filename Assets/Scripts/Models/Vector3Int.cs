using UnityEngine;

public class Vector3Int
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public Vector3Int(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3Int(Vector3 vector3)
    {
        X = Mathf.RoundToInt(vector3.x);
        Y = Mathf.RoundToInt(vector3.y);
        Z = Mathf.RoundToInt(vector3.z);
    }

    public Vector3Int() { }

    public static implicit operator Vector3(Vector3Int input)
    {
        return new Vector3(input.X, input.Y, input.Z);
    }

    public static explicit operator Vector3Int(Vector3 input)
    {
        return new Vector3Int(input);
    }
}
