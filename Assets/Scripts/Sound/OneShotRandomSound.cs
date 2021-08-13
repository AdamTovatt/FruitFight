using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OneShotRandomSound : MonoBehaviour
{
    public List<AudioClip> SoundClips;
    public AudioSource AudioSource;
    public bool PlayOnAwake = true;
    public bool PlayOnCollision = false;

    [Range(0f, 0.3f)]
    public float PitchVariation;

    void Start()
    {
        if (PlayOnAwake)
            Play();
    }

    private void Play()
    {
        AudioClip clip = SoundClips[Random.Range(0, SoundClips.Count)];

        if (PitchVariation > 0)
            AudioSource.pitch += (Random.Range(0f, PitchVariation) * (Random.Range(0, 101) > 50 ? 1 : -1));

        AudioSource.PlayOneShot(clip);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!AudioSource.isPlaying)
        {
            Debug.Log("Play sound");
            Play();
        }
    }
}
