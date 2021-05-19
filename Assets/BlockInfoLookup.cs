using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lookups
{
    public static class BlockInfoLookup
    {
        private static Dictionary<int, BlockInfo> blockInfoLookup;
        private static BlockInfoContainer blockInfoContainer;
        private static bool isInitialized = false;

        public static void Initialize()
        {
            if (!isInitialized)
            {
                blockInfoContainer = BlockInfoContainer.LoadFromConfiguration();
                blockInfoLookup = blockInfoContainer.CreateLookup();
                isInitialized = true;
            }
        }

        public static BlockInfoContainer GetBlockInfoContainer()
        {
            if (!isInitialized)
                Initialize();

            return blockInfoContainer;
        }

        public static BlockInfo Get(int blockInfoId)
        {
            if (!isInitialized)
                Initialize();

            if (!blockInfoLookup.ContainsKey(blockInfoId))
            {
                Debug.LogError("Missing key: " + blockInfoId + " in BlockInfoLookup");
                return null;
            }

            return blockInfoLookup[blockInfoId];
        }
    }
}