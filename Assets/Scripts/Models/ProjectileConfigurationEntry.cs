using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileConfigurationEntry
{
    public int Id;
    public string PrefabName;

    public GameObject Prefab { get { if (_prefab == null) Load(); return _prefab; } }
    private GameObject _prefab;

    private void Load()
    {
        _prefab = Resources.Load<GameObject>(string.Format("Prefabs/Magic/{0}", PrefabName));
    }
}
