using UnityEngine;

public class DestroyWithDelay : MonoBehaviour
{
    public float DelaySeconds = 1f;

    void Start()
    {
        this.CallWithDelay(Destroy, DelaySeconds);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
