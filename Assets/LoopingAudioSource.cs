using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingAudioSource : MonoBehaviour
{
    public List<AudioClip> Sounds;

    [Range(0f, 2f)]
    public float Volume = 0.3f;
    public float MinDistance = 1;
    public float MaxDistance = 30;

    private AudioSource audioSource;
    private float originalVolume;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.minDistance = MinDistance;
        audioSource.maxDistance = MaxDistance;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.spatialBlend = 1;

        SetRandomSound();

        originalVolume = audioSource.volume;
    }

    private void SetRandomSound()
    {
        audioSource.clip = Sounds[Random.Range(0, Sounds.Count)];
        audioSource.time = Random.Range(0, audioSource.clip.length);
    }

    public void StartPlaying()
    {
        audioSource.Play();
    }

    public void StopPlaying()
    {
        audioSource.Stop();
    }

    public void SetVolumeMultiplier(float volume)
    {
        Debug.Log("Volume multiplier: " + volume);
        audioSource.volume = volume * originalVolume;
    }
}
