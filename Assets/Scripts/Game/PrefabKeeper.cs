using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabKeeper : MonoBehaviour
{
    public static PrefabKeeper Instance;

    public List<Prefab> PrefabKeys;
    public List<GameObject> PrefabValues;

    private Dictionary<Prefab, GameObject> prefabs;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        prefabs = new Dictionary<Prefab, GameObject>();

        if (PrefabKeys.Count != PrefabValues.Count)
            throw new System.Exception("PrefabKeys doesn't match PrefabValues");

        for (int i = 0; i < PrefabKeys.Count; i++)
        {
            prefabs.Add(PrefabKeys[i], PrefabValues[i]);
        }

        DontDestroyOnLoad(gameObject);
    }

    public GameObject GetPrefab(Prefab? prefab)
    {
        if (prefab == null)
            return null;

        if (!prefabs.ContainsKey((Prefab)prefab))
        {
            Debug.LogError("No prefab " + prefab + " exists");
            return null;
        }
        return prefabs[(Prefab)prefab];
    }
}

public enum Prefab
{
    WizardHat, Beanie
}
