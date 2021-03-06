using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueSoundSourceManager : MonoBehaviour
{
    public static UniqueSoundSourceManager Instance;

    private Dictionary<string, List<UniqueSoundSource>> sources = new Dictionary<string, List<UniqueSoundSource>>();

    bool lastFrame = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (lastFrame)
        {
            lastFrame = false;
        }
        else
        {
            List<string> keysToRemove = null;

            foreach (string key in sources.Keys)
            {
                if (sources[key].Count > 0)
                {
                    if (sources[key][0] == null) //if this source doesn't exist anymore we want to add it to the list of keys to delete
                    {
                        if (keysToRemove == null)
                            keysToRemove = new List<string>();

                        keysToRemove.Add(key);
                    }
                    else //we only do this if the sources exists
                    {
                        UniqueSoundSource closest = sources[key][0];
                        float closestDistance = (transform.position - closest.transform.position).sqrMagnitude;
                        float distance;
                        foreach (UniqueSoundSource source in sources[key])
                        {
                            distance = (transform.position - source.transform.position).sqrMagnitude;
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closest = source;
                            }
                        }

                        foreach (UniqueSoundSource source in sources[key])
                        {
                            source.Deactivate();
                        }

                        closest.Activate();
                    }
                }
            }

            if (keysToRemove != null)
            {
                foreach (string key in keysToRemove) //remove all the sources that don't exist anymore
                    sources.Remove(key);
            }
        }
    }

    public void AddSoundSource(UniqueSoundSource uniqueSoundSource)
    {
        if (!sources.ContainsKey(uniqueSoundSource.Id))
            sources.Add(uniqueSoundSource.Id, new List<UniqueSoundSource>());

        sources[uniqueSoundSource.Id].Add(uniqueSoundSource);
    }

    public void RemoveSoundSource(UniqueSoundSource uniqueSoundSource)
    {
        if (sources.ContainsKey(uniqueSoundSource.Id))
            sources[uniqueSoundSource.Id].Remove(uniqueSoundSource);
    }
}
