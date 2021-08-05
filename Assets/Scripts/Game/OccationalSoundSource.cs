using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccationalSoundSource : MonoBehaviour
{
    public AudioClip[] Clips;
    public AudioSource AudioSource;

    public int MinChaining = 1;
    public int MaxChaining = 5;

    [Range(0, 100)]
    public float ProbabilityPerSecond = 0;

    private void Start()
    {
        if (!WorldBuilder.IsInEditor)
            AudioManager.Instance.OnSecondHasPassed += SecondPassed;
    }

    private void SecondPassed()
    {
        if (!AudioSource.isPlaying)
        {
            if (Random.Range(0, 1000) < ProbabilityPerSecond * 10)
            {
                AudioSource.clip = Clips[Random.Range(0, Clips.Length - 1)];
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
