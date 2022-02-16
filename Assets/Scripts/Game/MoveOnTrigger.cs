using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : ActivatedByStateSwitcher
{
    public Vector3 FinalPosition { get; set; }
    public float MoveSpeed { get; set; } = 0.5f;
    public float EndpointDelay { get; set; }
    public bool PingPong { get; set; }
    public bool LinearMovement { get; set; }

    public Vector3 CurrentPosition { get; set; }
    public bool Active { get { return active; } }

    public bool Moving { get { return lerping; } }

    public Vector3 CurrentMovement { get; private set; }
    public Vector3 CurrentVelocity { get; private set; }
    public AverageVelocityKeeper AverageVelocityKeeper { get; set; }

    private LoopingAudioSource loopingAudio;
    private BoardPlatform boardPlatform;

    private bool active = false;
    private bool lerping { get { return _lerping; } set { _lerping = value; LerpingWasSet(value); } }
    private bool _lerping;
    private float lerpValue = 0f;
    private Vector3 lastPosition;
    private float currentMoveSpeed = 0;
    private float isPingPongSoundMultiplier = 1f; //will be multiplied with the speed to set the volume of the looping sound, if it's pingpong the sound will be reduced to not get annoying

    private float initTime;

    private Coroutine coroutine;

    private List<PlayerMovement> players = new List<PlayerMovement>();

    public override void Init(Block thisBlock, Block activatorBlock)
    {
        if (gameObject.GetComponent<AverageVelocityKeeper>() == null)
        {
            AverageVelocityKeeper = gameObject.AddComponent<AverageVelocityKeeper>();
        }
        initTime = Time.time;
        block = thisBlock;
        activatorObject = activatorBlock;
        lastPosition = transform.position;

        loopingAudio = gameObject.GetComponent<LoopingAudioSource>();
        boardPlatform = gameObject.GetComponentInChildren<BoardPlatform>();

        CurrentPosition = transform.position;
    }

    public override void BindStateSwitcher()
    {
        if (PingPong)
        {
            isPingPongSoundMultiplier = 0.25f; //sounds that are on constantly are reduced to not get annoying
            Activated();
            return; //can't both ping pong and be activated
        }

        if (activatorObject != null && activatorObject.Instance != null)
        {
            stateSwitcher = activatorObject.Instance.GetComponent<StateSwitcher>();
            stateSwitcher.OnActivated += Activated;
            stateSwitcher.OnDeactivated += Deactivated;
        }
    }

    public override void Activated()
    {
        if (EndpointDelay > 0)
        {
            coroutine = StartCoroutine(ActivateInSeconds());
        }
        else
        {
            DoActivate();
        }
    }

    public override void Deactivated()
    {
        if (EndpointDelay > 0)
        {
            coroutine = StartCoroutine(DeActivateInSeconds());
        }
        else
        {
            DoDeActivate();
        }
    }

    private IEnumerator DeActivateInSeconds()
    {
        yield return new WaitForSeconds(EndpointDelay);
        coroutine = null;
        DoDeActivate();
    }

    private IEnumerator ActivateInSeconds()
    {
        yield return new WaitForSeconds(EndpointDelay);
        coroutine = null;
        DoActivate();
    }

    public void DoDeActivate()
    {
        if(boardPlatform != null && boardPlatform.IsMoving)
        {
            boardPlatform.QueueDeactivate();
            return;
        }

        active = false;
        lerping = true;
    }

    public void DoActivate()
    {
        if(boardPlatform != null && boardPlatform.IsMoving)
        {
            boardPlatform.QueueActivate();
            return;
        }

        active = true;
        lerping = true;

        if (!PingPong && IsAtStartOfLevel())
        {
            lerpValue = 1;
            transform.position = FinalPosition + block.RotationOffset;
            lastPosition = transform.position;
            lerping = false;
        }
    }

    private void Update()
    {
        if (lerping)
        {
            if (lerpValue <= 1 && lerpValue >= 0)
            {
                float lerpDelta = 0;

                float appliedMoveSpeed = MoveSpeed * (LinearMovement ? 1 : Mathf.Clamp((-(Mathf.Pow(((lerpValue * 2) - 1), 2))) + 1, 0.1f, 0.8f));
                currentMoveSpeed = appliedMoveSpeed;
                
                if (loopingAudio != null)
                    loopingAudio.SetVolumeMultiplier(Mathf.Clamp01(currentMoveSpeed) * isPingPongSoundMultiplier); //ping pong sounds are reduced to not get annoying

                if (active)
                {
                    lerpDelta = Time.deltaTime * appliedMoveSpeed;
                    CurrentVelocity = appliedMoveSpeed * ((FinalPosition + block.RotationOffset) - block.AppliedPosition).normalized;
                }
                else
                {
                    lerpDelta = -Time.deltaTime * appliedMoveSpeed;
                    CurrentVelocity = appliedMoveSpeed * (block.AppliedPosition - (FinalPosition + block.RotationOffset)).normalized;
                }

                lerpValue += lerpDelta;             

                Vector3 newPosition = Vector3.Lerp(block.Position + block.RotationOffset, FinalPosition + block.RotationOffset, lerpValue);
                CurrentMovement = newPosition - lastPosition;
                transform.position = newPosition;
                CurrentPosition = newPosition;
                lastPosition = transform.position;

            }
            else
            {
                if (lerpValue > 1)
                    transform.position = FinalPosition + block.RotationOffset;
                else if (lerpValue < 0)
                    transform.position = block.Position + block.RotationOffset;

                lerpValue = Mathf.Clamp(lerpValue, 0, 1);
                lerping = false;
                CurrentVelocity = Vector3.zero;

                if (PingPong)
                {
                    if (active == false)
                        Activated();
                    else
                        Deactivated();
                }
            }
        }
    }

    private void LateUpdate()
    {
        foreach(PlayerMovement movement in players)
        {
            Debug.Log("Updating player movement");
            movement.transform.position += CurrentMovement;
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

    private bool IsAtStartOfLevel()
    {
        return Time.time - initTime < 1.5f;
    }

    private void LerpingWasSet(bool newValue)
    {
        if (!IsAtStartOfLevel() && loopingAudio != null)
        {
            if (newValue)
                loopingAudio.StartPlaying();
            else
                loopingAudio.StopPlaying();
        }
    }

    private void OnDestroy()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        if (stateSwitcher != null)
        {
            stateSwitcher.OnActivated -= Activated;
            stateSwitcher.OnDeactivated -= Deactivated;
        }
    }
}
