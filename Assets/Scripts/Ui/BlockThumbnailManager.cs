using Lookups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockThumbnailManager : MonoBehaviour
{
    public GameObject BlockThumbnailScenePrefab;

    public static Dictionary<string, RawImage> BlockThumbnails { get; private set; }

    private void Start()
    {
        if (BlockThumbnails == null)
        {
            BlockThumbnails = new Dictionary<string, RawImage>();
            GenerateThumbnails();
        }
    }

    private void GenerateThumbnails()
    {
        BlockThumbnailScene scene = Instantiate(BlockThumbnailScenePrefab, new Vector3(0, 1000, 0), Quaternion.identity, transform).GetComponent<BlockThumbnailScene>();

        BlockInfoContainer blockInfoContainer = BlockInfoLookup.GetBlockInfoContainer();

        List<GameObject> prefabs = new List<GameObject>();

        foreach(BlockInfo info in blockInfoContainer.Infos)
        {
            prefabs.Add(PrefabLookup.GetPrefab(info.Prefab));
        }

        scene.CreateThumbnails(prefabs);
    }
}
