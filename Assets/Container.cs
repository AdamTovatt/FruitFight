using Lookups;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public enum ReleaseType
    {
        OnDestroyed, ByActivator
    }

    public class ContainerProperties : BehaviourProperties
    {
        [StringInput(CheckIfPrefab = true, Name = "Effect", Description = "The effect that will be used for when an object is released")]
        public string ReleaseEffectPrefab { get; set; }
        
        [StringInput(CheckIfPrefab = true, Name = "Object", Description = "The object that will be released")]
        public string ReleaseObjectPrefab { get; set; }
        
        [IntInput(MinValue = 1, MaxValue = 10, Name = "Amount", Description = "The amount of objects that will be released")]
        public int AmountToRelease { get; set; }
        
        [FloatInput(MinValue = 0.0f, MaxValue = 10f, Name = "Force strength", Description = "The force with which the objects that are released will be released. Objects that have a higher force will fly further.")]
        public float SpawnForceStrength { get; set; } = 5f;
        
        [EnumInput(EnumType = typeof(ReleaseType), Name = "Trigger type", Description = "The type of trigger that will trigger the release. Should the release happen when the releasing object is destroyed or should it be determined by some other trigger?")]
        public ReleaseType ReleaseType { get; set; }
        
        [ActivatorInput(Name = "Activator", Description = "The object that will give the trigger to release objects if that release type is chosen")]
        public int ActivatorObjectId { get; set; }

        [BoolInput(Name = "Multiple times", Description = "Should this container release objects multiple times or just once?")]
        public bool ReleaseMultipleTimes { get; set; }
    }

    public ContainerProperties Properties;

    private Collider objectCollider;
    private StateSwitcher stateSwitcher;
    private bool hasReleased;

    private GameObject releaseEffectPrefab;
    private GameObject releaseObjectPrefab;

    private void Awake()
    {
        objectCollider = gameObject.GetComponent<Collider>();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks += WorldWasBuilt;
    }

    private void UnBindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks -= WorldWasBuilt;

        if (stateSwitcher != null)
            stateSwitcher.OnActivated -= StateSwitcherActivated;
    }

    private void WorldWasBuilt()
    {
        Block activator = WorldBuilder.Instance.GetPlacedBlock(Properties.ActivatorObjectId);

        if (activator != null && activator.Instance != null)
        {
            stateSwitcher = activator.Instance.GetComponent<StateSwitcher>();

            if (stateSwitcher != null)
            {
                stateSwitcher.OnActivated += StateSwitcherActivated;
            }
        }
    }

    private void StateSwitcherActivated()
    {
        Release();

        if (!Properties.ReleaseMultipleTimes)
            UnBindEvents(); //if we aren't gonna release multiple times we will unbind the event now to avoid calling it when it isn't needed
    }

    public void Initialize(ContainerProperties containerProperties)
    {
        Properties = containerProperties;

        if (!string.IsNullOrEmpty(Properties.ReleaseObjectPrefab))
        {
            releaseObjectPrefab = PrefabLookup.GetPrefab(Properties.ReleaseObjectPrefab);
        }

        if (!string.IsNullOrEmpty(Properties.ReleaseEffectPrefab))
        {
            releaseEffectPrefab = PrefabLookup.GetPrefab(Properties.ReleaseEffectPrefab);
        }

        if (Properties.ReleaseType == ReleaseType.OnDestroyed)
        {
            Health health = gameObject.GetComponent<Health>();

            if (health != null)
                health.OnDied += WasDestroyed;
        }
        else
        {
            BindEvents();
        }
    }

    private void WasDestroyed(Health sender, CauseOfDeath causeOfDeath)
    {
        Release();
    }

    private void Release()
    {
        if (!Properties.ReleaseMultipleTimes && hasReleased)
            return; //if we should not release multiple times and have already released, don't release

        if (objectCollider != null)
            objectCollider.enabled = false;

        if (Properties.ReleaseObjectPrefab != null && CustomNetworkManager.HasAuthority)
        {
            for (int i = 0; i < Properties.AmountToRelease; i++)
            {
                GameObject spawnedObject = SpawnPrefab(releaseObjectPrefab);

                if (CustomNetworkManager.IsOnlineSession)
                    NetworkServer.Spawn(spawnedObject);
            }
        }

        if (Properties.ReleaseEffectPrefab != null && CustomNetworkManager.HasAuthority)
        {
            GameObject spawnedObject = SpawnPrefab(releaseEffectPrefab);

            if (CustomNetworkManager.IsOnlineSession)
                NetworkServer.Spawn(spawnedObject);
        }

        hasReleased = true;
    }

    private GameObject SpawnPrefab(GameObject prefab)
    {
        GameObject instantiatedObject = Instantiate(prefab, transform.position + (Vector3.up * 0.5f), Quaternion.identity);
        Rigidbody rigidbody = instantiatedObject.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.velocity = UnityEngine.Random.onUnitSphere * Properties.SpawnForceStrength + Vector3.up;
        }

        return instantiatedObject;
    }
}
