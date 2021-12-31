using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HatConfigurationEntry
{
    public int Id;
    public string DisplayName;
    public string PrefabName;
    public int MagicProjectileId;

    public GameObject Prefab { get { if (_prefab == null) Load(); return _prefab; } }
    private GameObject _prefab;

    private void Load()
    {
        _prefab = Resources.Load<GameObject>(string.Format("Prefabs/Hats/{0}", PrefabName));
    }
}
