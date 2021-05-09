using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBananaMan : MonoBehaviour
{
    private GameObject currentHat;
    private Transform hatPoint;
    private Transform bananaManTransform;

    void Start()
    {
        hatPoint = gameObject.GetComponentInChildren<HatPoint>().transform;
        bananaManTransform = gameObject.GetComponentInChildren<Rotate>().transform;
    }

    public void SetHat(Prefab prefab)
    {
        if (currentHat != null)
            Destroy(currentHat);

        currentHat = Instantiate(PrefabKeeper.Instance.GetPrefab(prefab), hatPoint.position, bananaManTransform.rotation);
        currentHat.transform.SetParent(hatPoint);
    }
}
