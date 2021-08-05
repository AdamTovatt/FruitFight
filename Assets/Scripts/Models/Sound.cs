using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Sound
{
    public string Name;
    public AudioClip[] Clips;

    [Range(0f, 2f)]
    public float Volume = 1;
    [Range(0.1f, 3f)]
    public float Pitch = 1;
    [Range(0f, 0.3f)]
    public float PitchVariation = 0;

    public AudioSource Source { get; private set; }

    public void SetSource(AudioSource source)
    {
        source.clip = Clips[0];
        source.volume = Volume;
        source.pitch = Pitch;

        Source = source;
    }

    public void SetRandomClip()
    {
        Source.clip = Clips[Random.Range(0, Clips.Length - 1)];
    }

    public void Play(float pitchShift)
    {
        Source.pitch = Pitch + (Random.Range(0, PitchVariation) * (Random.Range(0, 100) > 50 ? 1 : -1)) + pitchShift;
        Source.Play();
    }
}
