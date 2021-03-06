using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class World
{
    public List<Block> Blocks { get; set; }

    private WorldBlockLookup blockLookup;

    public WorldMetadata Metadata;
    public float TimeOfDay = 15;
    public float NorthRotation = 180;

    public static int LastBlockId { get; set; }

    [JsonIgnore]
    public StoryModeLevelEntry StoryModeLevelEntry { get; set; }

    public World()
    {
        blockLookup = new WorldBlockLookup();
        Blocks = new List<Block>();
    }

    public void Add(Block block)
    {
        Blocks.Add(block);
        blockLookup.Add(block);
    }

    public void Remove(Block block, Vector3Int position)
    {
        Blocks = Blocks.Where(x => x.Id != block.Id).ToList();
        blockLookup.Remove(block, position);
    }

    public List<Block> GetBlocksAtPosition(Vector3Int position)
    {
        return blockLookup.GetBlocksAtPosition(position);
    }

    public static World FromJson(string json)
    {
        World world = JsonConvert.DeserializeObject<World>(json);
        world.blockLookup.Initialize(world.Blocks);
        world.CalculateNeighbors();

        if (world.Blocks.Count > 0)
            LastBlockId = world.Blocks.OrderByDescending(b => b.Id).First().Id;

        return world;
    }

    public static World FromWorldName(string worldName)
    {
        try
        {
            string json = WorldUtilities.LoadTextFile(string.Format("Maps/{0}", worldName));
            return FromJson(json);
        }
        catch(NullReferenceException)
        {
            Debug.LogError(worldName + " does not exist in the assets folder (probably)");
            throw;
        }
    }

    public void CalculateNeighbors()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            Blocks[i].CalculateNeighbors(this);
        }
    }

    public string ToJson(bool prettyPrint = false)
    {
        return JsonConvert.SerializeObject(this, prettyPrint ? Formatting.Indented : Formatting.None);
    }
}
