using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    public AudioSource AudioSourceSettings;
    public List<Sound> Sounds;
    public bool PlayOnAwake = false;

    private Dictionary<string, Sound> soundDictionary;

    private void Awake()
    {
        soundDictionary = new Dictionary<string, Sound>();

        foreach (Sound sound in Sounds)
        {
            sound.SetSource(gameObject.AddComponent<AudioSource>(), AudioSourceSettings);
            soundDictionary.Add(sound.Name, sound);
        }
    }

    public void StartPlaying(string soundName)
    {
        Sound sound = soundDictionary[soundName];
        sound.Source.clip = sound.GetRandomClip();
        sound.Source.loop = true;
        sound.Play();
    }

    public void StopPlaying(string soundName)
    {
        Sound sound = soundDictionary[soundName];
        sound.Source.loop = false;
        sound.Source.Stop();
    }

    public void Play(string soundName)
    {
        Sound sound = soundDictionary[soundName];
        sound.SetRandomClip();
        sound.Play();
    }
}
