using Lookups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockThumbnailManager : MonoBehaviour
{
    public GameObject BlockThumbnailScenePrefab;

    public static Dictionary<string, Texture2D> BlockThumbnails { get; private set; }

    public delegate void ThumbnailsCreatedHandler(object sender, Dictionary<string, Texture2D> createdThumbnails);
    public event ThumbnailsCreatedHandler OnThumbnailsCreated;

    private void Start()
    {
        if (BlockThumbnails == null)
        {
            BlockThumbnails = new Dictionary<string, Texture2D>();
            GenerateThumbnails();
        }
        else
        {
            OnThumbnailsCreated?.Invoke(this, BlockThumbnails);
        }
    }

    private void GenerateThumbnails()
    {
        BlockThumbnailScene scene = Instantiate(BlockThumbnailScenePrefab, new Vector3(0, 1000, 0), Quaternion.identity, transform).GetComponent<BlockThumbnailScene>();

        BlockInfoContainer blockInfoContainer = BlockInfoLookup.GetBlockInfoContainer();

        Dictionary<string, BlockInfo> prefabs = new Dictionary<string, BlockInfo>();

        foreach(BlockInfo info in blockInfoContainer.Infos)
        {
            prefabs.Add(info.Prefab, info);
        }

        scene.OnThumbnailCreationCompleted += (sender, thumbnails) => 
        { 
            BlockThumbnails = thumbnails; 
            Destroy(scene);
            OnThumbnailsCreated?.Invoke(this, BlockThumbnails);
        };

        scene.CreateThumbnails(prefabs);
    }
}
