using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehaviour : MonoBehaviour
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
    }

    public MoveBehaviourProperties Properties;
    public bool Moving { get { return moveState != MoveState.Still; } }
    public Vector3 CurrentMovement { get; set; }

    private float speed;
    private Vector3 positionalDifference;
    private StateSwitcher stateSwitcher;
    private float lerpValue;
    private MoveState moveState = MoveState.Still;
    private List<PlayerMovement> players = new List<PlayerMovement>();

    private bool hasSufficientValues;

    private Vector3 positionA;
    private Vector3 positionB;

    public void Initialize(BehaviourProperties behaviourProperties)
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
        speed = positionalDifference.sqrMagnitude;

        BindEvents();
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

        if (!WorldBuilder.IsInEditor || WorldEditor.IsTestingLevel)
            transform.position = positionA;
    }

    private void StateSwitcherActivated()
    {
        moveState = MoveState.TowardsB;
        CurrentMovement = positionalDifference / Properties.MoveTime;
    }

    private void StateSwitcherDeactivated()
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
            transform.position = positionA + positionalDifference * lerpValue;

            if (lerpValue == 1 || lerpValue == 0)
            {
                moveState = MoveState.Still;
                CurrentMovement = Vector3.zero;
            }
        }
    }

    private void LateUpdate()
    {
        foreach (PlayerMovement movement in players)
        {
            Debug.Log("Updating player movement");
            movement.transform.position += CurrentMovement / 800f;
        }
    }

    public void AddPlayer(PlayerMovement player)
    {
        players.Add(player);
    }

    public void RemovePlayer(PlayerMovement player)
    {
        players.Remove(player);
    }
}
