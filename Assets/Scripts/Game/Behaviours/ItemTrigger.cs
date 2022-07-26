using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : BehaviourBase
{
    public enum TriggerItemType
    {
        Coin, JellyBean, Key
    }

    public class ItemTriggerProperties : BehaviourProperties
    {
        [EnumInput(EnumType = typeof(TriggerItemType), Name = "Item", Description = "The item that needs to be put in this trigger to activate it")]
        public TriggerItemType ItemType { get; set; }

        [IntInput(MinValue = 1, MaxValue = 10000, Name = "Item amount", Description = "The amount of items needed to activate the trigger")]
        public int Amount { get; set; }

        public override Type BehaviourType { get { return typeof(ItemTrigger); } }
    }

    public ItemTriggerProperties Properties { get; set; }
    public bool IsSatisfied { get; set; }

    public BlockInformationHolder BlockInfo { get; set; }

    private StateSwitcher stateSwitcher;
    private GameObject worldIconPrefab;

    private WorldIcon worldIcon;

    private void Awake()
    {
        stateSwitcher = gameObject.AddComponent<StateSwitcher>();
        worldIconPrefab = Resources.Load<GameObject>("Prefabs/WorldIcon");
    }

    private void Start()
    {
        worldIcon = Instantiate(worldIconPrefab, transform, false).GetComponent<WorldIcon>();
        worldIcon.SetupIcon(Properties.ItemType.ToString(), Properties.Amount.ToString());
    }

    public override void Initialize(BehaviourProperties behaviourProperties)
    {
        Properties = (ItemTriggerProperties)behaviourProperties;
        BlockInfo = gameObject.GetComponent<BlockInformationHolder>();
    }

    public void WasSatisfied()
    {
        stateSwitcher.Activate();
        worldIcon.gameObject.SetActive(false);
        IsSatisfied = true;
    }
}
