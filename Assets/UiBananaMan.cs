using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBananaMan : MonoBehaviour
{
    private GameObject currentHat;
    private Transform hatPoint;

    void Start()
    {
        hatPoint = gameObject.GetComponentInChildren<HatPoint>().transform;
    }

    public void SetHat(Prefab prefab)
    {
        if (currentHat != null)
            Destroy(currentHat);

        currentHat = Instantiate(PrefabKeeper.Instance.GetPrefab(prefab), hatPoint.position, hatPoint.rotation, hatPoint);
    }
}
