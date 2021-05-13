using UnityEngine;

public class OnlyShowIfDebug : MonoBehaviour
{
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = gameObject.GetComponent<Renderer>();
        objectRenderer.enabled = false;
        
        if (GameManager.Instance.IsDebug)
            objectRenderer.enabled = true;

        GameManager.Instance.OnDebugStateChanged += (gameManager, newState) => { objectRenderer.enabled = newState; };
    }
}
