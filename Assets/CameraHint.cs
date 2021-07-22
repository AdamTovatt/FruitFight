using UnityEngine;

public class CameraHint : MonoBehaviour
{
    public Transform SphereGraphic;

    public float Radius { get { return _radius; } set { _radius = value; _radiusSquared = value * value; } }
    private float _radius;
    public float RadiusSquared { get { return _radiusSquared; } }
    private float _radiusSquared;

    private SphereCollider sphereCollider;

    private void Start()
    {
        sphereCollider = gameObject.GetComponent<SphereCollider>();
        SetRadius(12);
    }

    public void SetRadius(float radius)
    {
        Radius = radius;
        sphereCollider.radius = radius;
        SphereGraphic.localScale = new Vector3(Radius * 2, Radius * 2, Radius * 2);
    }
}
