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

    /// <summary>
    /// If the object should wobble on collision, requires a wobble component on the object!
    /// </summary>
    public bool WobbleOnCollision = false;

    private Wobble wobble;

    [Range(0f, 0.3f)]
    public float PitchVariation;

    private void Awake()
    {
        if(WobbleOnCollision)
        {
            wobble = gameObject.GetComponent<Wobble>();
        }
    }

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
            Play();

            if (WobbleOnCollision && wobble != null)
                wobble.StartWobble();
        }
    }
}
