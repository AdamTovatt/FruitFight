using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OneShotRandomSound : MonoBehaviour
{
    public List<AudioClip> SoundClips;
    public AudioSource AudioSource;
    
    [Range(0f, 0.3f)]
    public float PitchVariation;

    void Start()
    {
        AudioClip clip = SoundClips[Random.Range(0, SoundClips.Count - 1)];

        if (PitchVariation > 0)
            AudioSource.pitch += (Random.Range(0, PitchVariation) * (Random.Range(0, 100) > 50 ? 1 : -1));

        AudioSource.PlayOneShot(clip);
    }
}
