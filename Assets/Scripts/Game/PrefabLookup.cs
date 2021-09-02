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
                            LoadPrefab(prefab);
                        }
                    }

                    loadedBlocks.Add(blockInfo.Id);
                }
            }
        }

        private static void LoadPrefab(string name)
        {
            prefabLookup.Add(name, Resources.Load<GameObject>(string.Format("Prefabs/Terrain/{0}", name)));
        }

        public static GameObject GetPrefab(string name)
        {
            if (!isInitialized)
                Initialize();

            if (!prefabLookup.ContainsKey(name))
            {
                Debug.LogError("The prefab: " + name + " has not been loaded correctly.");
                LoadPrefab(name);
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