using UnityEngine;

public class GroundedPositionInformation
{
    public Transform Transform { get; set; }
    public float Time { get; set; }
    public Vector3 Position { get; set; }

    public GroundedPositionInformation(Transform transform, float time, Vector3 position)
    {
        Transform = transform;
        Time = time;
        Position = position;
    }

    public void UpdatePosition(Vector3 position)
    {
        Time = UnityEngine.Time.time;
        Position = position;
    }
}
