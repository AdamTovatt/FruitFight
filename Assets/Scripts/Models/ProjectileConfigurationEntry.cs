using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileConfigurationEntry
{
    public int Id;
    public string ProjectilePrefabName;
    public string ChargePrefabName;
    public string HitPrefabName;

    public GameObject Projectile { get { if (_projectile == null) Load(); return _projectile; } }
    private GameObject _projectile;

    public GameObject Charge { get { if (_charge == null) Load(); return _charge; } }
    private GameObject _charge;

    public GameObject Hit { get { if (_hit == null) Load(); return _hit; } }
    private GameObject _hit;

    public void Load()
    {
        _projectile = Resources.Load<GameObject>(string.Format("Prefabs/Magic/{0}", ProjectilePrefabName));
        _charge = Resources.Load<GameObject>(string.Format("Prefabs/Magic/{0}", ChargePrefabName));
        _hit = Resources.Load<GameObject>(string.Format("Prefabs/Magic/{0}", HitPrefabName));

        if (_projectile == null)
            Debug.LogError("Error when loading projectile with name: " + ProjectilePrefabName);
        if (_charge == null)
            Debug.LogError("Error when loading (projectile-)charge with name: " + ChargePrefabName);
        if (_hit == null)
            Debug.LogError("Error when loading (projectile-)hit with name: " + HitPrefabName);
    }
}
