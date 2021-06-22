using UnityEngine;

public class SeeThroughBlock : MonoBehaviour
{
    public MeshRenderer SeeThroughMesh;

    public bool Enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            if (value)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
    }

    private bool _enabled = false;
    private float enableTime;

    private void Start()
    {
        Disable();
    }

    private void Update()
    {
        if (_enabled)
        {
            if (Time.time - enableTime > 0.05f)
            {
                Disable();
            }
        }
    }

    public void Enable()
    {
        SeeThroughMesh.enabled = true;
        _enabled = true;
        enableTime = Time.time;
    }

    public void Disable()
    {
        SeeThroughMesh.enabled = false;
        _enabled = false;
    }
}
