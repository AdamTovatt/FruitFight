using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGate : MonoBehaviour
{
    public Transform Spikes;
    public float UpHeight;
    public float DownHeight;

    private Activatable activatable;
    private float lerpValue;
    private bool lerping;
    private Vector3 startPosition;
    private float heightDifference;

    private void Start()
    {
        activatable = gameObject.GetComponent<Activatable>();

        if (activatable != null)
            BindEvents();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        if (activatable != null)
            activatable.OnActivated += GoDown;
    }

    private void UnBindEvents()
    {
        if (activatable != null)
            activatable.OnActivated -= GoDown;
    }

    private void GoDown()
    {
        lerpValue = 0;
        lerping = true;
        startPosition = Spikes.localPosition;
        heightDifference = startPosition.y - DownHeight;
        Debug.Log("startheight: " + startPosition.y);
    }

    private void Update()
    {
        if (lerping)
        {
            lerpValue += Time.deltaTime / 3f;

            lerpValue = Mathf.Clamp(lerpValue, 0f, 1f);

            Spikes.localPosition = new Vector3(startPosition.x, startPosition.y - heightDifference * lerpValue, startPosition.z);

            if (lerpValue >= 1)
                lerping = false;
        }
    }
}
