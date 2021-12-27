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
    private List<Block> currentBlocks = new List<Block>();

    private World upcommingWorld;
    private List<ActivatedByStateSwitcher> activatedByStateSwitcherObjectsToBind = new List<ActivatedByStateSwitcher>();
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
        currentBlocks.Clear();

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
        {
            BindObjects();
            //OptimizeObjects(); //doesn't work
        }
        else
        {
            foreach (AlwaysFaceCamera faceCamera in FindObjectsOfType<AlwaysFaceCamera>())
            {
                faceCamera.Activate();
            }
        }

        CurrentWorld = world;

        if (DaylightController.Instance != null)
        {
            DaylightController.Instance.Initialize(world.NorthRotation, world.TimeOfDay);
        }
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
                    GameObject water = Instantiate(PrefabLookup.GetPrefab("Water"), new Vector3(x * width, -2f, y * width), Quaternion.identity, transform);
                    water.GetComponent<MeshRenderer>().enabled = false;
                    previousWorldObjects.Add(water);
                }
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

                //add the behaviour property collections that should exist from start on those kinds of blocks that have behaviours from start

                DetailColorController detailColor = block.Instance.GetComponent<DetailColorController>();
                if (detailColor != null)
                {
                    block.BehaviourProperties.DetailColorPropertyCollection = new DetailColorPropertyCollection();
                }

                TriggerZone triggerZone = block.Instance.GetComponent<TriggerZone>();
                if (triggerZone != null)
                {
                    block.BehaviourProperties.TriggerZonePropertyCollection = new TriggerZonePropertyCollection();
                    block.BehaviourProperties.TriggerZonePropertyCollection.IsParent = true;
                    block.BehaviourProperties.TriggerZonePropertyCollection.HasValues = true;
                }

                NotificationBlock notificationBlock = block.Instance.GetComponent<NotificationBlock>();
                if (notificationBlock != null)
                {
                    block.BehaviourProperties.NotificationPropertyCollection = new NotificationPropertyCollection();
                }
            }

            if (block.BehaviourProperties.MovePropertyCollection != null && block.BehaviourProperties.MovePropertyCollection.HasValues) //init move
            {
                MoveOnTrigger moveOnTrigger = block.Instance.GetComponent<MoveOnTrigger>();
                if (moveOnTrigger == null)
                    moveOnTrigger = block.Instance.AddComponent<MoveOnTrigger>();

                moveOnTrigger.Init(block, upcommingWorld.Blocks.Where(b => b.Id == block.BehaviourProperties.MovePropertyCollection.ActivatorBlockId).FirstOrDefault());
                activatedByStateSwitcherObjectsToBind.Add(moveOnTrigger);

                propertyExposer.Behaviours.Add(moveOnTrigger);
                block.MakeEnforceEdges();
            }

            if (block.BehaviourProperties.TriggerZonePropertyCollection != null && block.BehaviourProperties.TriggerZonePropertyCollection.HasValues) //init trigger zone
            {
                TriggerZone triggerZone = block.Instance.GetComponent<TriggerZone>();

                triggerZone.Init(block.BehaviourProperties.TriggerZonePropertyCollection.IsParent, GetBlockInUpcommingWorld(block.BehaviourProperties.TriggerZonePropertyCollection.ParentId));
                propertyExposer.Behaviours.Add(triggerZone);

                triggerZoneObjectsToBind.Add(triggerZone);
            }

            if (block.BehaviourProperties.NotificationPropertyCollection != null && block.BehaviourProperties.NotificationPropertyCollection.HasValues) //init notifications
            {
                NotificationBlock notificationBlock = block.Instance.GetComponent<NotificationBlock>();

                notificationBlock.Init(block, GetBlockInUpcommingWorld(block.BehaviourProperties.NotificationPropertyCollection.ActivatorBlockId));
                activatedByStateSwitcherObjectsToBind.Add(notificationBlock);
            }

            propertyExposer.WasLoaded(block.BehaviourProperties);
        }

        if (block.Info.BlockType == BlockType.Block && block.Info.EdgePrefabs.Count > 0) //edges of blocks
        {
            System.Random random = new System.Random(block.Position.GetSumOfComponents());

            float halfSideLenght = (float)block.Info.Width / 2f;

            if (block.EnforceEdge || block.NeighborX.SameTypesPositive == null || block.NeighborX.SameTypesPositive.Where(b => b.Info.Width >= block.Info.Width).Count() < 1) //if we don't have a neighbor we should create an edge
            {
                Vector3 edgePosition = new Vector3(block.Position.X + halfSideLenght, block.Position.Y - 0.001f, block.Position.Z + halfSideLenght);
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, 90, 0), block.Instance.transform));
            }

            if (block.EnforceEdge || block.NeighborX.SameTypesNegative == null || block.NeighborX.SameTypesNegative.Where(b => b.Info.Width >= block.Info.Width).Count() < 1)
            {
                Vector3 edgePosition = new Vector3(block.Position.X + halfSideLenght, block.Position.Y - 0.001f, block.Position.Z + halfSideLenght);
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, -90, 0), block.Instance.transform));
            }

            if (block.EnforceEdge || block.NeighborZ.SameTypesPositive == null || block.NeighborZ.SameTypesPositive.Where(b => b.Info.Width >= block.Info.Width).Count() < 1)
            {
                Vector3 edgePosition = new Vector3(block.Position.X + halfSideLenght, block.Position.Y - 0.001f, block.Position.Z + halfSideLenght);
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, 0, 0), block.Instance.transform));
            }

            if (block.EnforceEdge || block.NeighborZ.SameTypesNegative == null || block.NeighborZ.SameTypesNegative.Where(b => b.Info.Width >= block.Info.Width).Count() < 1)
            {
                Vector3 edgePosition = new Vector3(block.Position.X + halfSideLenght, block.Position.Y - 0.001f, block.Position.Z + halfSideLenght);
                previousWorldObjects.Add(Instantiate(PrefabLookup.GetPrefab(block.Info.EdgePrefabs, random), edgePosition, Quaternion.Euler(0, 180, 0), block.Instance.transform));
            }
        }

        if (block.Instance != null)
        {
            block.InformationHolder = block.Instance.AddComponent<BlockInformationHolder>();
            block.InformationHolder.Block = block;
            currentBlocks.Add(block);
        }
    }

    private Block GetBlockInUpcommingWorld(int blockId)
    {
        return upcommingWorld.Blocks.Where(b => b.Id == blockId).FirstOrDefault();
    }

    private void OptimizeObjects() //an attempt at making a method that optimizes the objects by static batching, for some reason some of the objects get a yellowish tint
    {
        Dictionary<int, List<Block>> blocksByTypes = new Dictionary<int, List<Block>>();
        foreach(Block block in currentBlocks)
        {
            if (!block.InformationHolder.CanMove && (block.Info.Id == 18 || block.Info.Id == 22 || block.Info.Id == 33))
            {
                if (!blocksByTypes.ContainsKey(block.Info.Id))
                {
                    blocksByTypes.Add(block.Info.Id, new List<Block>());
                }

                blocksByTypes[block.Info.Id].Add(block);
            }
        }

        foreach(List<Block> blocks in blocksByTypes.Values)
        {
            Debug.Log("Blocks to combine: " + blocks.Count);

            if(blocks.Count > 1)
            {
                List<GameObject> gameObjects = new List<GameObject>();
                foreach(Block block in blocks)
                {
                    foreach (Transform childTransform in block.Instance.GetComponentsInChildren<Transform>())
                    {
                        gameObjects.Add(childTransform.gameObject);
                        childTransform.gameObject.isStatic = true;
                    }
                }

                foreach(GameObject child in gameObjects)
                {
                    child.transform.SetParent(blocks.First().Instance.transform);
                }

                StaticBatchingUtility.Combine(blocks.First().Instance);
            }
        }
    }

    private void BindObjects()
    {
        foreach (ActivatedByStateSwitcher activatedByStateSwitcher in activatedByStateSwitcherObjectsToBind)
        {
            activatedByStateSwitcher.BindStateSwitcher();

            if (activatedByStateSwitcher.GetType() == typeof(MoveOnTrigger)) //to make objects on top of moving objects also move
            {
                BlockInformationHolder blockInformationHolder = activatedByStateSwitcher.GetComponent<BlockInformationHolder>();

                if (blockInformationHolder != null)
                {
                    blockInformationHolder.CanMove = true;

                    foreach (Block neighbor in blockInformationHolder.Block.NeighborY.AllTypesNegative)
                    {
                        BlockInformationHolder neighborInformationHolder = neighbor.Instance.transform.GetComponent<BlockInformationHolder>();
                        if(neighborInformationHolder != null)
                        {
                            neighborInformationHolder.CanMove = true;
                        }

                        if (neighbor.Info.BlockType == BlockType.Detail || neighbor.Info.BlockType == BlockType.Prop)
                        {
                            neighbor.Instance.transform.parent = blockInformationHolder.transform;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Missing BlockInformationHolder on MoveOnTrigger object");
                }
            }
        }

        foreach (TriggerZone zone in triggerZoneObjectsToBind)
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
