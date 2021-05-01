using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagnetAttractor : MonoBehaviour
{
    public float SphereOfInfluenceRadius = 3f;
    public Vector3 AttractionPointOffset = Vector3.zero;

    public Vector3 AttractionPoint { get { return transform.position + AttractionPointOffset; } }

    private void Start()
    {
        
    }

    private void Update()
    {
        List<Magnet> magnets = CheckForMagnets();

        foreach(Magnet magnet in magnets)
        {
            if (!magnet.Targets.Contains(this))
                magnet.Targets.Add(this);
        }
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
}
