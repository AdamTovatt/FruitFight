using Assets.Scripts.Models;
using Lookups;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public static WorldBuilder Instance;
    public static World NextLevel;
    public static bool IsInEditor = false;

    public IReadOnlyList<GameObject> CurrentPlacedObjects { get { return previousWorldObjects; } }

    private List<GameObject> previousWorldObjects;

    private World upcommingWorld;
    private List<MoveOnTrigger> moveOnTriggerObjectsToBind = new List<MoveOnTrigger>();
    private List<TriggerZone> triggerZoneObjectsToBind = new List<TriggerZone>();
    public World CurrentWorld { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        previousWorldObjects = new List<GameObject>();
    }

    public void BuildNext()
    {
        if (NextLevel == null)
            NextLevel = World.FromWorldName("01");

        BuildWorld(NextLevel);
    }

    public void Build(string worldName)
    {
        World world = World.FromWorldName(worldName);
        BuildWorld(world);
    }

    public void BuildWorld(World world)
    {
        upcommingWorld = world;

        debugCubes.Clear();

        foreach (GameObject gameObject in previousWorldObjects)
        {
            Destroy(gameObject);
        }

        foreach (Block block in world.Blocks)
        {
            block.Instance = null;
            PlaceBlock(block);
        }

        if (!IsInEditor)
            BindObjects();

        CurrentWorld = world;
    }

    List<PartialBlockIntersection> debugCubes = new List<PartialBlockIntersection>();
    public void PlaceBlock(Block block)
    {
        if (block.Info.BlockType == BlockType.Block || block.Info.BlockType == BlockType.Invisible || block.Info.BlockType == BlockType.Prop || block.Info.BlockType == BlockType.Detail)
        {
            GameObject instantiatedObject = Instantiate(PrefabLookup.GetPrefab(block.Info.Prefab), block.Position + block.RotationOffset, Quaternion.identity, transform);
            block.Instance = instantiatedObject;
            block.SeeThroughBlock = instantiatedObject.GetComponent<SeeThroughBlock>();
            previousWorldObjects.Add(instantiatedObject);
            if (block.Info.RotatableX || block.Info.RotatableY)
            {
                block.Instance.transform.rotation = block.Rotation;
            }
        }
        else if (block.Info.BlockType == BlockType.Ocean && !IsInEditor)
        {
            int width = block.Info.Width;
            for (int x = -5; x < 5; x++)
            {
                for (int y = -5; y < 5; y++)
                {
                    previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab("Water"), new Vector3(x * width, -2f, y * width), Quaternion.identity, transform));
                }
            }
        }

        if (block.Info.BlockType == BlockType.Block && block.Info.EdgePrefabs.Count > 0)
        {
            System.Random random = new System.Random(block.Position.GetSumOfComponents());

            float halfSideLenght = (float)block.Info.Width / 2f;

            if (block.NeighborX.Positive == null || block.NeighborX.Positive.Where(b => b.Info.Width >= block.Info.Width).Count() < 1) //if we don't have a neighbor we should create an edge
            {
                Vector3 edgePosition = new Vector3(block.Position.X + halfSideLenght, block.Position.Y - 0.001f, block.Position.Z + halfSideLenght);
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, 90, 0), block.Instance.transform));
            }

            if (block.NeighborX.Negative == null || block.NeighborX.Negative.Where(b => b.Info.Width >= block.Info.Width).Count() < 1)
            {
                Vector3 edgePosition = new Vector3(block.Position.X + halfSideLenght, block.Position.Y - 0.001f, block.Position.Z + halfSideLenght);
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, -90, 0), block.Instance.transform));
            }

            if (block.NeighborZ.Positive == null || block.NeighborZ.Positive.Where(b => b.Info.Width >= block.Info.Width).Count() < 1)
            {
                Vector3 edgePosition = new Vector3(block.Position.X + halfSideLenght, block.Position.Y - 0.001f, block.Position.Z + halfSideLenght);
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, 0, 0), block.Instance.transform));
            }
            if (block.NeighborZ.Negative == null || block.NeighborZ.Negative.Where(b => b.Info.Width >= block.Info.Width).Count() < 1)
            {
                Vector3 edgePosition = new Vector3(block.Position.X + halfSideLenght, block.Position.Y - 0.001f, block.Position.Z + halfSideLenght);
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, 180, 0), block.Instance.transform));
            }
        }

        if (!block.HasPropertyExposer && block.Info.StartWithPropertyExposer)
            block.HasPropertyExposer = true;

        if (block.HasPropertyExposer)
        {
            PropertyExposer propertyExposer = block.Instance.GetComponent<PropertyExposer>();

            if (propertyExposer == null)
            {
                propertyExposer = block.Instance.AddComponent<PropertyExposer>();
            }

            if (block.BehaviourProperties == null)
            {
                block.BehaviourProperties = new BehaviourPropertyContainer();

                DetailColorController detailColor = block.Instance.GetComponent<DetailColorController>();
                if (detailColor != null)
                {
                    block.BehaviourProperties.DetailColorPropertyCollection = new DetailColorPropertyCollection();
                }
            }

            if (block.BehaviourProperties.MovePropertyCollection != null && block.BehaviourProperties.MovePropertyCollection.HasValues)
            {
                MoveOnTrigger moveOnTrigger = block.Instance.GetComponent<MoveOnTrigger>();
                if (moveOnTrigger == null)
                    moveOnTrigger = block.Instance.AddComponent<MoveOnTrigger>();

                moveOnTrigger.Init(block, upcommingWorld.Blocks.Where(b => b.Id == block.BehaviourProperties.MovePropertyCollection.ActivatorBlockId).FirstOrDefault());
                moveOnTriggerObjectsToBind.Add(moveOnTrigger);

                propertyExposer.Behaviours.Add(moveOnTrigger);
            }

            if(block.BehaviourProperties.TriggerZonePropertyCollection != null && block.BehaviourProperties.TriggerZonePropertyCollection.HasValues)
            {
                TriggerZone triggerZone = block.Instance.GetComponent<TriggerZone>();

                triggerZone.Init(block.BehaviourProperties.TriggerZonePropertyCollection.IsParent, upcommingWorld.Blocks.Where(b => b.Id == block.BehaviourProperties.TriggerZonePropertyCollection.ParentId).FirstOrDefault());
                propertyExposer.Behaviours.Add(triggerZone);

                triggerZoneObjectsToBind.Add(triggerZone);
            }

            propertyExposer.WasLoaded(block.BehaviourProperties);
        }
    }

    private void BindObjects()
    {
        foreach (MoveOnTrigger move in moveOnTriggerObjectsToBind)
        {
            move.BindStateSwitcher();
        }

        foreach(TriggerZone zone in triggerZoneObjectsToBind)
        {
            zone.Bind();
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (PartialBlockIntersection cube in debugCubes)
        {
            Gizmos.DrawCube(cube.CenterPoint, cube.Size);
        }
    }

    public void AddPreviousWorldObjects(GameObject gameObject)
    {
        previousWorldObjects.Add(gameObject);
    }
}
