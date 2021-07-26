using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBananaMan : MonoBehaviour
{
    private GameObject currentHat;
    private Transform hatPoint;
    private Transform bananaManTransform;

    public Rotate Rotate { get { return rotate; } }
    private Rotate rotate;

    void Start()
    {
        hatPoint = gameObject.GetComponentInChildren<HatPoint>().transform;
        rotate = gameObject.GetComponentInChildren<Rotate>();
        bananaManTransform = rotate.transform;
    }

    public void SetHat(Prefab? prefab)
    {
        if (currentHat != null)
            Destroy(currentHat);

        if (prefab != null)
        {
            currentHat = Instantiate(PrefabKeeper.Instance.GetPrefab((Prefab)prefab), hatPoint.position, bananaManTransform.rotation);
            currentHat.transform.SetParent(hatPoint);
        }
    }
}
