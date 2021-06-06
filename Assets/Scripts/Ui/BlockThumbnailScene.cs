using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BlockThumbnailScene : MonoBehaviour
{
    public Camera ThumbnailCamera;

    public delegate void ThumbnailCreationCompletedHandler(object sender, Dictionary<string, Texture2D> createdThumbnails);
    public event ThumbnailCreationCompletedHandler OnThumbnailCreationCompleted;

    private Dictionary<string, GameObject> actorQueue;

    private GameObject currentActor;
    private int cyclesWithoutActors = 0;

    private Dictionary<string, Texture2D> createdThumbnails;

    private void Awake()
    {
        createdThumbnails = new Dictionary<string, Texture2D>();
        actorQueue = new Dictionary<string, GameObject>();
    }

    public void Update()
    {
        if (actorQueue.Count == 0)
        {
            if (createdThumbnails.Count == 0)
            {
                cyclesWithoutActors++;
                if (cyclesWithoutActors > 5)
                    Destroy(gameObject);
            }
            else
            {
                OnThumbnailCreationCompleted?.Invoke(this, createdThumbnails);
            }
        }
        else
        {
            if (currentActor == null)
                StartCoroutine(CreateNextThumbnail());
        }
    }

    public void CreateThumbnails(Dictionary<string, GameObject> prefabs)
    {
        foreach (string key in prefabs.Keys)
        {
            if (!actorQueue.ContainsKey(key))
                actorQueue.Add(key, prefabs[key]);
        }
    }

    private IEnumerator CreateNextThumbnail()
    {
        if (currentActor != null)
        {
            Debug.LogError("Previous actor has not been cleaned up");
            yield return null;
        }

        string prefabKey = actorQueue.Keys.First();

        List<Renderer> renderers = new List<Renderer>();

        if (actorQueue[prefabKey] != null)
        {
            currentActor = Instantiate(actorQueue[prefabKey], transform.position, transform.rotation, transform);
            currentActor.layer = 7;

            Renderer renderer = currentActor.GetComponent<Renderer>();
            if (renderer != null)
                renderers.Add(renderer);

            foreach (Renderer child in currentActor.GetComponentsInChildren<Renderer>())
            {
                renderers.Add(child);
                child.gameObject.layer = 7;
            }
        }

        foreach(Renderer renderer in renderers)
        {
            
        }

        actorQueue.Remove(prefabKey);

        yield return new WaitForEndOfFrame();

        RenderTexture.active = ThumbnailCamera.targetTexture;
        Texture2D renderResult = new Texture2D(ThumbnailCamera.targetTexture.width, ThumbnailCamera.targetTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, ThumbnailCamera.targetTexture.width, ThumbnailCamera.targetTexture.height);
        renderResult.ReadPixels(rect, 0, 0);
        renderResult.Apply();

        createdThumbnails.Add(prefabKey, renderResult);

        Destroy(currentActor);
    }
}
