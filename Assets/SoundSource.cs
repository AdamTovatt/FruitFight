using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSource : MonoBehaviour
{
    public List<Sound> Sounds;
    public bool PlayOnAwake = false;

    private Dictionary<string, Sound> soundDictionary;
    private AudioSource audioSource;

    private void Awake()
    {
        soundDictionary = new Dictionary<string, Sound>();
        audioSource = gameObject.GetComponent<AudioSource>();

        foreach (Sound sound in Sounds)
        {
            sound.SetSource(gameObject.AddComponent<AudioSource>());
            soundDictionary.Add(sound.Name, sound);
        }
    }

    public void StartPlaying(string soundName)
    {
        Sound sound = soundDictionary[soundName];
        audioSource.clip = sound.GetRandomClip();
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopPlaying(string soundName)
    {
        audioSource.loop = false;
        audioSource.Stop();
    }

    public void Play(string soundName)
    {
        Sound sound = soundDictionary[soundName];
        audioSource.PlayOneShot(sound.GetRandomClip());
    }
}
