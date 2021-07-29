using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : MonoBehaviour
{
    public Vector3 FinalPosition { get; set; }
    public int ActivatorBlockId { get; set; }

    private Block activatorObject;
    private Block block;

    private StateSwitcher stateSwitcher;

    private bool active = false;
    private bool lerping = false;
    private float lerpValue = 0f;
    private float moveSpeed = 0.5f;

    private float initTime;

    public void Init(Block thisBlock, Block activatorBlock)
    {
        initTime = Time.time;
        block = thisBlock;
        activatorObject = activatorBlock;
    }

    public void BindStateSwitcher()
    {
        stateSwitcher = activatorObject.Instance.GetComponent<StateSwitcher>();
        stateSwitcher.OnActivated += Activated;
        stateSwitcher.OnDeactivated += Deactivated;
    }

    public void Activated()
    {
        active = true;
        lerping = true;

        if (Time.time - initTime < 1.5f)
        {
            lerpValue = 1;
            transform.position = FinalPosition + block.RotationOffset;
            lerping = false;
        }
    }

    public void Deactivated()
    {
        active = false;
        lerping = true;
        transform.position = block.Position + block.RotationOffset;
    }

    private void Update()
    {
        if (lerping)
        {
            if (lerpValue <= 1 && lerpValue >= 0)
            {
                if (active)
                    lerpValue += Time.deltaTime * moveSpeed * Mathf.Clamp((1 - lerpValue), 0.1f, 1f);
                else
                    lerpValue -= Time.deltaTime * moveSpeed * Mathf.Clamp(lerpValue, 0.1f, 1f);

                transform.position = Vector3.Lerp(block.Position + block.RotationOffset, FinalPosition + block.RotationOffset, lerpValue);
            }
            else
            {
                if (lerpValue > 1)
                    transform.position = FinalPosition + block.RotationOffset;
                else if (lerpValue < 0)
                    transform.position = block.Position + block.RotationOffset;

                lerpValue = Mathf.Clamp(lerpValue, 0, 1);
                lerping = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (stateSwitcher != null)
        {
            stateSwitcher.OnActivated -= Activated;
            stateSwitcher.OnDeactivated -= Deactivated;
        }
    }
}
