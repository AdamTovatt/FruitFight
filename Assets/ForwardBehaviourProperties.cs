using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardBehaviourProperties : MonoBehaviour
{
    public Spawner Spawner;
    public Transform BehaviourHolder;

    private void Awake()
    {
        Spawner.OnObjectSpawned += Spawned;
    }

    private void Spawned(object sender, GameObject spawnedObject)
    {
        Spawner.OnObjectSpawned -= Spawned;

        BlockInformationHolder blockInformationHolder = BehaviourHolder.gameObject.GetComponent<BlockInformationHolder>();
        if (blockInformationHolder == null)
        {
            Debug.LogError("Could not forward behaviour of spawned object: " + transform.name);
            return;
        }

        Block block = blockInformationHolder.Block;

        if (block.BehaviourProperties2 != null && block.BehaviourProperties2.Count > 0)
            WorldBuilder.BindBehaviourProperties(block.BehaviourProperties2, spawnedObject);
    }
}
