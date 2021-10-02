using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnetAttractor : MonoBehaviour
{
    public float SphereOfInfluenceRadius = 3f;
    public Vector3 AttractionPointOffset = Vector3.zero;

    public Vector3 AttractionPoint { get { return transform.position + AttractionPointOffset; } }

    public static List<MagnetAttractor> MagnetAttractors;

    private void Awake()
    {
        if (MagnetAttractors == null)
            MagnetAttractors = new List<MagnetAttractor>();
    }

    private void Start()
    {
        MagnetAttractors.Add(this);
    }

    private List<Magnet> CheckForMagnets()
    {
        List<Magnet> result = new List<Magnet>();
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, SphereOfInfluenceRadius, Vector3.up);

        foreach(RaycastHit hit in hits)
        {
            if (hit.transform.tag == "Magnet")
            {
                Magnet magnet = hit.transform.GetComponent<Magnet>();
                if (magnet != null)
                    result.Add(magnet);
            }
        }

        return result;
    }

    private void OnDestroy()
    {
        MagnetAttractors = MagnetAttractors.Where(x => x != this).ToList();
    }
}
