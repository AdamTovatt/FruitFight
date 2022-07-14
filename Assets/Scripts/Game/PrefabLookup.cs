using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lookups
{
    public static class PrefabLookup
    {
        private static Dictionary<string, GameObject> prefabLookup;
        private static bool isInitialized = false;

        public static void Initialize()
        {
            if (!isInitialized)
            {
                prefabLookup = new Dictionary<string, GameObject>();
                LoadPrefabs(BlockInfoLookup.GetBlockInfoContainer());
                LoadAdditionalPrefabs();
                isInitialized = true;
            }
        }

        private static void LoadPrefabs(BlockInfoContainer blockInfoContainer)
        {
            List<int> loadedBlocks = new List<int>();

            foreach (BlockInfo blockInfo in blockInfoContainer.Infos)
            {
                if (!loadedBlocks.Contains(blockInfo.Id))
                {
                    List<string> prefabNames = new List<string>() { blockInfo.Prefab };
                    prefabNames.AddRange(blockInfo.EdgePrefabs);

                    foreach (string prefab in prefabNames)
                    {
                        if (!prefabLookup.ContainsKey(prefab))
                        {
                            LoadPrefab(prefab, "Prefabs/Terrain/{0}");
                        }
                    }

                    loadedBlocks.Add(blockInfo.Id);
                }
            }
        }

        private static void LoadAdditionalPrefabs()
        {
            foreach(PrefabConfigurationEntry prefab in PrefabConfiguration.GetInstance().Prefabs)
            {
                try
                {
                    LoadPrefab(prefab.Name, prefab.Path);
                }
                catch(Exception exception)
                {
                    Debug.LogError("Error when loading prefab: " + prefab + " " + exception.Message);
                }
            }
        }

        private static void LoadPrefab(string name, string path)
        {
            prefabLookup.Add(name, Resources.Load<GameObject>(string.Format(path, name)));
        }

        public static bool PrefabExists(string name)
        {
            if (!isInitialized)
                Initialize();

            return prefabLookup.ContainsKey(name);
        }

        public static GameObject GetPrefab(string name)
        {
            if (!isInitialized)
                Initialize();

            if (!prefabLookup.ContainsKey(name))
            {
                Debug.LogError("The prefab: " + name + " has not been loaded correctly.");
                //LoadPrefab(name); this should not be here but I don't want to remove it because it might introduce a bug, you can remove it if you havent noticed any bugs related to prefabs
            }

            return prefabLookup[name];
        }

        public static GameObject GetPrefab(List<string> nameVariations, System.Random random)
        {
            if (nameVariations.Count == 0)
                return null;

            return GetPrefab(nameVariations[random.Next(0, nameVariations.Count)]);
        }
    }
}