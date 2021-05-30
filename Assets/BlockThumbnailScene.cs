using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BlockThumbnailScene : MonoBehaviour
{
    public Camera ThumbnailCamera;

    public delegate void ThumbnailCreationCompletedHandler(object sender, List<Texture2D> createdThumbnails);
    public event ThumbnailCreationCompletedHandler OnThumbnailCreationCompleted;

    private List<GameObject> actorQueue;

    private GameObject currentActor;
    private int cyclesWithoutActors = 0;

    private List<Texture2D> createdThumbnails;

    private void Awake()
    {
        createdThumbnails = new List<Texture2D>();
        actorQueue = new List<GameObject>();
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

    public void CreateThumbnails(List<GameObject> prefabs)
    {
        actorQueue.AddRange(prefabs);
    }

    private IEnumerator CreateNextThumbnail()
    {
        if (currentActor != null)
        {
            Debug.LogError("Previous actor has not been cleaned up");
            yield return null;
        }

        GameObject prefab = actorQueue[0];

        if (prefab != null)
        {
            currentActor = Instantiate(prefab, transform.position, transform.rotation, transform);
            currentActor.layer = 7;

            foreach(Renderer child in currentActor.GetComponentsInChildren<Renderer>())
            {
                child.gameObject.layer = 7;
            }
        }

        actorQueue = actorQueue.Where(x => x != prefab).ToList();

        yield return new WaitForEndOfFrame();

        RenderTexture.active = ThumbnailCamera.targetTexture;
        Texture2D renderResult = new Texture2D(ThumbnailCamera.targetTexture.width, ThumbnailCamera.targetTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, ThumbnailCamera.targetTexture.width, ThumbnailCamera.targetTexture.height);
        renderResult.ReadPixels(rect, 0, 0);
        renderResult.Apply();

        byte[] bytes = renderResult.EncodeToPNG();
        System.IO.File.WriteAllBytes(string.Format("C:\\users\\adam\\desktop\\test_{0}.png", createdThumbnails.Count), bytes);
        Debug.Log("Screenshot");

        createdThumbnails.Add(renderResult);

        Destroy(currentActor);
    }
}
