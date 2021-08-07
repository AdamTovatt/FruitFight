using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueSoundSource : MonoBehaviour
{
    public string Id;

    public bool Active { get { return active; } }
    private bool active;

    private void Start()
    {
        UniqueSoundSourceManager.Instance.AddSoundSource(this);
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
    }

    private void OnDestroy()
    {
        UniqueSoundSourceManager.Instance.RemoveSoundSource(this);
    }
}
