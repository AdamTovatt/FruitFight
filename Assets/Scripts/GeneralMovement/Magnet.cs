using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float SphereOfInfluenceRadius = 3f;
    public float Strength = 5f;
    public float MaxSpeed = 6f;

    public List<MagnetAttractor> Targets { get; private set; }

    private float sphereOfInfluenceRadiusSquared;

    private void Start()
    {
        sphereOfInfluenceRadiusSquared = SphereOfInfluenceRadius * SphereOfInfluenceRadius;
        Targets = new List<MagnetAttractor>();
        transform.tag = "Magnet";

        Targets = MagnetAttractor.MagnetAttractors;
    }

    private void Update()
    {
        if (Targets == null || Targets.Count == 0)
            return;

        foreach (MagnetAttractor target in Targets.Where(x => (x.transform.position - transform.position).sqrMagnitude < sphereOfInfluenceRadiusSquared))
        {
            Vector3 distance = transform.position - target.AttractionPoint;
            transform.position = Vector3.MoveTowards(transform.position, target.AttractionPoint, Mathf.Min(MaxSpeed, Strength / (distance.sqrMagnitude / 2)) * Time.deltaTime);
        }
    }
}
