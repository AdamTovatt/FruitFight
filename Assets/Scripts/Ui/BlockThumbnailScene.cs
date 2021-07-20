using Lookups;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BlockThumbnailScene : MonoBehaviour
{
    public Camera ThumbnailCamera;
    public Light[] Lights;

    public delegate void ThumbnailCreationCompletedHandler(object sender, Dictionary<string, Texture2D> createdThumbnails);
    public event ThumbnailCreationCompletedHandler OnThumbnailCreationCompleted;

    private Dictionary<string, BlockInfo> actorQueue;

    private GameObject currentActor;
    private int cyclesWithoutActors = 0;

    private Dictionary<string, Texture2D> createdThumbnails;

    private void Awake()
    {
        createdThumbnails = new Dictionary<string, Texture2D>();
        actorQueue = new Dictionary<string, BlockInfo>();
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
                foreach (Light light in Lights)
                    light.enabled = false;

                OnThumbnailCreationCompleted?.Invoke(this, createdThumbnails);
            }
        }
        else
        {
            if (currentActor == null)
                StartCoroutine(CreateNextThumbnail());
        }
    }

    public void CreateThumbnails(Dictionary<string, BlockInfo> prefabs)
    {
        foreach (Light light in Lights)
            light.enabled = true;

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
        BlockInfo currentBlockInfo = actorQueue[prefabKey];

        List<Renderer> renderers = new List<Renderer>();

        if (currentBlockInfo != null)
        {
            currentActor = Instantiate(PrefabLookup.GetPrefab(currentBlockInfo.Prefab), transform.position, transform.rotation, transform);
            currentActor.layer = 7;

            if (currentBlockInfo.BlockType == BlockType.BuildingBlock && currentBlockInfo.EdgePrefabs.Count > 0)
            {
                System.Random random = new System.Random(0);

                float halfSideLenght = (float)currentBlockInfo.Width / 2f;

                Vector3 edgePosition = new Vector3(transform.position.x + halfSideLenght, transform.position.y - 0.001f, transform.position.z + halfSideLenght);
                Instantiate(PrefabLookup.GetPrefab(currentBlockInfo.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, 90, 0), currentActor.transform);

                Vector3 edgePosition1 = new Vector3(transform.position.x + halfSideLenght, transform.position.y - 0.001f, transform.position.z + halfSideLenght);
                Instantiate(PrefabLookup.GetPrefab(currentBlockInfo.EdgePrefabs, random), edgePosition1, Quaternion.Euler(0, -90, 0), currentActor.transform);

                Vector3 edgePosition2 = new Vector3(transform.position.x + halfSideLenght, transform.position.y - 0.001f, transform.position.z + halfSideLenght);
                Instantiate(PrefabLookup.GetPrefab(currentBlockInfo.EdgePrefabs, random), edgePosition2, Quaternion.Euler(0, 0, 0), currentActor.transform);

                Vector3 edgePosition3 = new Vector3(transform.position.x + halfSideLenght, transform.position.y - 0.001f, transform.position.z + halfSideLenght);
                Instantiate(PrefabLookup.GetPrefab(currentBlockInfo.EdgePrefabs, random), edgePosition3, Quaternion.Euler(0, 180, 0), currentActor.transform);
            }

            Renderer renderer = currentActor.GetComponent<Renderer>();
            if (renderer != null)
                renderers.Add(renderer);

            foreach (Renderer child in currentActor.GetComponentsInChildren<Renderer>())
            {
                renderers.Add(child);
                child.gameObject.layer = 7;
            }
        }

        float maxX = 0;
        float maxY = 0;
        float maxZ = 0;

        float centerX = 0;
        float centerY = 0;
        float centerZ = 0;

        foreach (Renderer renderer in renderers)
        {
            Vector3 size = renderer.bounds.size;

            if (size.x > maxX)
            {
                maxX = size.x;
                centerX = renderer.bounds.center.x;
            }
            if (size.y > maxY)
            {
                maxY = size.y;
                centerY = renderer.bounds.center.y;
            }
            if (size.z > maxZ)
            {
                maxZ = size.z;
                centerZ = renderer.bounds.center.z;
            }
        }

        float maxBounds = new Vector3(maxX, maxY, maxZ).magnitude;

        actorQueue.Remove(prefabKey);

        ThumbnailCamera.orthographicSize = maxBounds * 0.5f;
        ThumbnailCamera.transform.LookAt(new Vector3(centerX, centerY, centerZ));

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
