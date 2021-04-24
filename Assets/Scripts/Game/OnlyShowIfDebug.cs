using UnityEngine;

public class OnlyShowIfDebug : MonoBehaviour
{
    private Renderer renderer;

    void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        renderer.enabled = false;
        
        if (GameManager.Instance.IsDebug)
            renderer.enabled = true;

        GameManager.Instance.OnDebugStateChanged += (gameManager, newState) => { renderer.enabled = newState; };
    }
}
