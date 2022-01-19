using Lookups;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BlockThumbnailManager : MonoBehaviour
{
    public GameObject BlockThumbnailScenePrefab;

    public static Dictionary<string, Texture2D> BlockThumbnails { get; private set; }

    private List<string> loadedThumbnails = new List<string>();

    public delegate void ThumbnailsCreatedHandler(object sender, Dictionary<string, Texture2D> createdThumbnails);
    public event ThumbnailsCreatedHandler OnThumbnailsCreated;

    private void Start()
    {
        if (BlockThumbnails == null)
        {
            BlockThumbnails = new Dictionary<string, Texture2D>();
            LoadThumbnails();
            GenerateThumbnails();
        }
        else
        {
            OnThumbnailsCreated?.Invoke(this, BlockThumbnails);
        }
    }

    private void SaveThumbnailsIfEditor()
    {
        if (!Application.isEditor)
            return;

        string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string sakurPath = Path.Combine(appdataPath, "Sakur");
        string generatedPath = Path.Combine(sakurPath, "GeneratedThumbnails");

        if (!Directory.Exists(sakurPath))
            Directory.CreateDirectory(sakurPath);
        if (!Directory.Exists(generatedPath))
            Directory.CreateDirectory(generatedPath);

        string[] fileNames = Directory.GetFiles(generatedPath);

        foreach (string block in BlockThumbnails.Keys)
        {
            if (!loadedThumbnails.Contains(block))
            {
                string fileName = block + ".png";

                if (!fileNames.Contains(fileName))
                    File.WriteAllBytes(Path.Combine(generatedPath, fileName), BlockThumbnails[block].EncodeToPNG());
            }
        }
    }

    private void LoadThumbnails()
    {
        foreach (BlockInfo info in BlockInfoLookup.GetBlockInfoContainer().Infos)
        {
            Texture2D texture = Resources.Load<Texture2D>(string.Format("BlockThumbnails/{0}", info.Prefab));

            if (texture != null)
            {
                BlockThumbnails.Add(info.Prefab, texture);
                loadedThumbnails.Add(info.Prefab);
            }
        }
    }

    private void GenerateThumbnails()
    {
        BlockInfoContainer blockInfoContainer = BlockInfoLookup.GetBlockInfoContainer();

        Dictionary<string, BlockInfo> prefabs = new Dictionary<string, BlockInfo>();

        foreach (BlockInfo info in blockInfoContainer.Infos)
        {
            if (!BlockThumbnails.ContainsKey(info.Prefab)) //only if we haven't loaded this yet
                prefabs.Add(info.Prefab, info);
        }

        if (prefabs.Count > 0)
        {
            BlockThumbnailScene scene = Instantiate(BlockThumbnailScenePrefab, new Vector3(0, 1000, 0), Quaternion.identity, transform).GetComponent<BlockThumbnailScene>();

            scene.OnThumbnailCreationCompleted += (sender, thumbnails) =>
            {
                GenerationCompleted(sender, thumbnails, scene);
            };

            scene.CreateThumbnails(prefabs);
        }
        else
        {
            GenerationCompleted(null, null, null);
        }
    }

    private void GenerationCompleted(object sender, Dictionary<string, Texture2D> thumbnails, BlockThumbnailScene scene)
    {
        if (thumbnails != null)
        {
            foreach (string key in thumbnails.Keys)
            {
                if (!BlockThumbnails.ContainsKey(key))
                    BlockThumbnails.Add(key, thumbnails[key]);
            }
        }

        if (scene != null)
        {
            SaveThumbnailsIfEditor(); //we know that we have not generated any new thumbnails so we don't need to save them
            Destroy(scene);
        }

        OnThumbnailsCreated?.Invoke(this, BlockThumbnails);
    }
}
