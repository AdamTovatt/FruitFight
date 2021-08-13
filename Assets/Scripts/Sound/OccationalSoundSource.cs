using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccationalSoundSource : MonoBehaviour
{
    public AudioClip[] Clips;
    public AudioSource AudioSource;

    [Range(0, 100)]
    public float ProbabilityPerSecond = 0;

    private void Start()
    {
        BindAudioManagerSecondTick();
    }

    public void BindAudioManagerSecondTick()
    {
        if (!WorldBuilder.IsInEditor)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.OnSecondHasPassed += SecondPassed;
        }
    }

    private void SecondPassed()
    { 
        if(AudioSource == null)
        {
            OnDestroy();
            return;
        }

        if (!AudioSource.isPlaying)
        {
            if (Random.Range(0, 1000) < ProbabilityPerSecond * 10)
            {
                AudioSource.clip = Clips[Random.Range(0, Clips.Length)];
                AudioSource.PlayDelayed(Random.Range(0, 1.4f));
            }
        }
    }

    private void OnDestroy()
    {
        if (!WorldBuilder.IsInEditor)
            AudioManager.Instance.OnSecondHasPassed -= SecondPassed;
    }
}
