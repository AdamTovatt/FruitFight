using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepAudioSource : MonoBehaviour
{
    [Range(200, 255)]
    public int Priority = 200;
    [Range(0f, 2f)]
    public float Volume = 1;
    public float AudioSourceRange = 30f;
    [Range(-1f, 1f)]
    public float GlobalPitchShift = 0;
    [Range(1, 3)]
    public int SoundCountMultiplier = 1; //will use the soundlist this many times, this is to create more audiosources to decrease the risk of a clip being cut of by it starting to play again
    public List<Sound> Sounds;

    private void Awake()
    {
        List<Sound> sounds = new List<Sound>();

        for (int i = 0; i < SoundCountMultiplier; i++)
        {
            sounds.AddRange(Sounds);
        }

        Sounds = sounds;

        foreach (Sound sound in Sounds)
        {
            sound.Pitch += GlobalPitchShift;
            sound.SetSource(gameObject.AddComponent<AudioSource>());
            sound.Source.spatialBlend = 1;
            sound.Source.maxDistance = AudioSourceRange;
            sound.Source.minDistance = 1;
            sound.Source.priority = Priority;
            sound.Source.volume = Volume;
        }
    }

    public void PlayNext()
    {
        GetPlayableSound().Source.Play();
    }

    private Sound GetPlayableSound()
    {
        return Sounds[Random.Range(0, Sounds.Count - 1)];
    }
}
