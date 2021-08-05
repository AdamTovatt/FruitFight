using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : MonoBehaviour
{
    public Vector3 FinalPosition { get; set; }
    public int ActivatorBlockId { get; set; }
    public float MoveSpeed { get; set; } = 0.5f;
    public float EndpointDelay { get; set; }
    public bool PingPong { get; set; }
    public bool LinearMovement { get; set; }

    public Vector3 CurrentMovement { get; private set; }
    public AverageVelocityKeeper AverageVelocityKeeper { get; set; }

    private Block activatorObject;
    private Block block;

    private StateSwitcher stateSwitcher;

    private bool active = false;
    private bool lerping = false;
    private float lerpValue = 0f;
    private Vector3 lastPosition;

    private float initTime;

    Coroutine coroutine;

    public void Init(Block thisBlock, Block activatorBlock)
    {
        if (gameObject.GetComponent<AverageVelocityKeeper>() == null)
        {
            AverageVelocityKeeper = gameObject.AddComponent<AverageVelocityKeeper>();
        }
        initTime = Time.time;
        block = thisBlock;
        activatorObject = activatorBlock;
        lastPosition = transform.position;
    }

    public void BindStateSwitcher()
    {
        if (PingPong)
        {
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

    public void Activated()
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

    public void Deactivated()
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

    private void DoDeActivate()
    {
        active = false;
        lerping = true;
    }

    private void DoActivate()
    {
        active = true;
        lerping = true;

        if (!PingPong && Time.time - initTime < 1.5f)
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

                if (active)
                    lerpDelta = Time.deltaTime * MoveSpeed * (LinearMovement ? 1 : Mathf.Clamp((-(Mathf.Pow(((lerpValue * 2) - 1), 2))) + 1, 0.1f, 0.8f));
                else
                    lerpDelta = -Time.deltaTime * MoveSpeed * (LinearMovement ? 1 : Mathf.Clamp((-(Mathf.Pow(((lerpValue * 2) - 1), 2))) + 1, 0.1f, 0.8f));

                lerpValue += lerpDelta;

                Vector3 newPosition = Vector3.Lerp(block.Position + block.RotationOffset, FinalPosition + block.RotationOffset, lerpValue);
                CurrentMovement = newPosition - lastPosition;
                transform.position = newPosition;
                lastPosition = transform.position;
                
                Vector3 positionDifference = (block.Position + block.RotationOffset) - (FinalPosition + block.RotationOffset);
                CurrentMovement = positionDifference * lerpDelta;
            }
            else
            {
                if (lerpValue > 1)
                    transform.position = FinalPosition + block.RotationOffset;
                else if (lerpValue < 0)
                    transform.position = block.Position + block.RotationOffset;

                lerpValue = Mathf.Clamp(lerpValue, 0, 1);
                lerping = false;

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