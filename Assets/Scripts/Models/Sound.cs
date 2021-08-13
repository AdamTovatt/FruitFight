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
    public float Delay = 0;

    public AudioSource Source { get; private set; }

    public void SetSource(AudioSource source, AudioSource settings = null)
    {
        source.clip = Clips[0];
        source.volume = Volume;
        source.pitch = Pitch;

        if(settings != null)
        {
            source.minDistance = settings.minDistance;
            source.maxDistance = settings.maxDistance;
            source.spatialBlend = settings.spatialBlend;
            source.spread = settings.spread;
            source.rolloffMode = settings.rolloffMode;
        }

        Source = source;
    }

    public void SetRandomClip()
    {
        Source.clip = GetRandomClip();
    }

    public AudioClip GetRandomClip()
    {
        return Clips[Random.Range(0, Clips.Length)];
    }

    public void Play()
    {
        Play(0, Delay);
    }

    public void Play(float pitchShift, float delay = 0)
    {
        Source.pitch = Pitch + (Random.Range(0, PitchVariation) * (Random.Range(0, 101) > 50 ? 1 : -1)) + pitchShift;
        Source.PlayDelayed(delay);
    }
}
