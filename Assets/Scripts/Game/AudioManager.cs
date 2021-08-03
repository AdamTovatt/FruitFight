using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    public static AudioManager Instance { get; set; }

    public delegate void SecondHasPassedHandler();
    public event SecondHasPassedHandler OnSecondHasPassed;

    private Dictionary<string, Sound> soundDictionary;
    private float lastSecondTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);

        soundDictionary = new Dictionary<string, Sound>();

        foreach (Sound sound in Sounds)
        {
            sound.SetSource(gameObject.AddComponent<AudioSource>());
            soundDictionary.Add(sound.Name, sound);
        }
    }

    private void Start()
    {
        OnSecondHasPassed?.Invoke();
        lastSecondTime = Time.time;
    }

    private void Update()
    {
        if(Time.time - lastSecondTime > 1)
        {
            OnSecondHasPassed?.Invoke();
            lastSecondTime = Time.time;
        }
    }

    public void Play(string soundName)
    {
        Play(soundName, 0);
    }

    public void Play(string soundName, float pitchShift)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.LogError("No sound name found: " + soundName);
            return;
        }

        Sound sound = soundDictionary[soundName];

        if (sound.Clips.Length > 0)
            sound.SetRandomClip();

        sound.Source.pitch = sound.Pitch + (Random.Range(0, sound.PitchVariation) * (Random.Range(0, 100) > 50 ? 1 : -1)) + pitchShift;
        sound.Source.Play();
    }
}
