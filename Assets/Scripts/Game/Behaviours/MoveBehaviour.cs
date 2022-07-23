using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehaviour : BehaviourBase
{
    private enum MoveState
    {
        Still, TowardsB, TowardsA
    }

    public class MoveBehaviourProperties : BehaviourProperties
    {
        [PositionInput(Description = "The start position of the object", Name = "Position A")]
        public Vector3Int PositionA { get; set; }

        [PositionInput(Description = "The end position of the object", Name = "Position B")]
        public Vector3Int PositionB { get; set; }

        [FloatInput(Description = "The time it will take for the object to reach the other endpoint", MaxValue = 60.0f, MinValue = 0.01f, Name = "Move time")]
        public float MoveTime { get; set; } = 5;

        [FloatInput(Description = "The time that the object will be at an endpoint", MaxValue = 10.0f, MinValue = 0.0f, Name = "Endpoint delay")]
        public float EndpointDelay { get; set; }

        [BoolInput(Description = "If the object should move between the two endpoints all the time or not", Name = "Ping-pong")]
        public bool PingPong { get; set; }

        [ActivatorInput(Description = "The activator which will activate this movement", Name = "Activator")]
        public int ActivatorObjectId { get; set; }

        public override Type BehaviourType { get { return typeof(MoveBehaviour); } }
    }

    public MoveBehaviourProperties Properties { get; set; }
    public bool Moving { get { return moveState != MoveState.Still; } }
    public Vector3 CurrentMovement { get; set; }
    public Rigidbody Rigidbody { get; set; }

    private Vector3 positionalDifference;
    private StateSwitcher stateSwitcher;
    private float lerpValue;
    private MoveState moveState = MoveState.Still;

    private bool hasSufficientValues;

    private Vector3 positionA;
    private Vector3 positionB;

    public override void Initialize(BehaviourProperties behaviourProperties)
    {
        Properties = (MoveBehaviourProperties)behaviourProperties;

        if (Properties.PositionB != null)
        {
            positionB = Properties.PositionB;
            hasSufficientValues = true;
        }

        if (Properties.PositionA != null)
            positionA = Properties.PositionA;
        else
            positionA = transform.position;

        positionalDifference = positionB - positionA;

        Rigidbody = gameObject.AddComponent<Rigidbody>();
        Rigidbody.isKinematic = true;

        foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
        {
            Rigidbody rigidbody = collider.gameObject.GetComponent<Rigidbody>();

            if (rigidbody == null)
            {
                rigidbody = collider.gameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
            }

            NonSlidingRigidbody.Rigidbodies.TryAdd(collider.transform, Rigidbody);
        }

        BindEvents();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks += WorldWasBuilt;
        WorldBuilder.Instance.OnFinishedPlacingBlocksLate += WorldWasBuiltLate;
    }

    private void UnBindEvents()
    {
        WorldBuilder.Instance.OnFinishedPlacingBlocks -= WorldWasBuilt;
        WorldBuilder.Instance.OnFinishedPlacingBlocksLate -= WorldWasBuiltLate;

        if (stateSwitcher != null)
        {
            stateSwitcher.OnActivated -= StateSwitcherActivated;
            stateSwitcher.OnDeactivated -= StateSwitcherDeactivated;
        }
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
                stateSwitcher.OnDeactivated += StateSwitcherDeactivated;
            }
        }
    }

    private void WorldWasBuiltLate()
    {
        if (!WorldBuilder.IsInEditor || WorldEditor.IsTestingLevel)
        {
            transform.position = positionA;

            if (Properties.PingPong && stateSwitcher == null)
                StateSwitcherActivated();
        }
    }

    private void StateSwitcherActivated()
    {
        if (moveState == MoveState.Still) //if the block is still it should wait for the endpoint delay
        {
            this.CallWithDelay(() => StartMoveTowardsB(), Properties.EndpointDelay);
        }
        else
        {
            StartMoveTowardsB();
        }
    }

    private void StartMoveTowardsB()
    {
        moveState = MoveState.TowardsB;
        CurrentMovement = positionalDifference / Properties.MoveTime;
    }

    private void StateSwitcherDeactivated()
    {
        if (moveState == MoveState.Still) //if the block is still it should wait for the endpoint delay
        {
            this.CallWithDelay(() => StartMoveTowardsA(), Properties.EndpointDelay);
        }
        else
        {
            StartMoveTowardsA();
        }
    }

    private void StartMoveTowardsA()
    {
        moveState = MoveState.TowardsA;
        CurrentMovement = positionalDifference / -Properties.MoveTime;
    }

    private void Update()
    {
        if (hasSufficientValues && moveState != MoveState.Still)
        {
            if (moveState == MoveState.TowardsB)
            {
                lerpValue += Time.deltaTime / Properties.MoveTime;
            }
            else
            {
                lerpValue -= Time.deltaTime / Properties.MoveTime;
            }

            lerpValue = Mathf.Clamp(lerpValue, 0f, 1f);
            //transform.position = positionA + positionalDifference * lerpValue;
            Rigidbody.MovePosition(positionA + positionalDifference * lerpValue);

            if (lerpValue == 1 || lerpValue == 0)
            {
                moveState = MoveState.Still;
                CurrentMovement = Vector3.zero;

                if (Properties.PingPong)
                {
                    if (lerpValue == 1)
                        StateSwitcherDeactivated();
                    else if (lerpValue == 0)
                        StateSwitcherActivated();
                }
            }
        }
    }
}
