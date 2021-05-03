using System.Collections.Generic;
using UnityEngine;

public static class CustomPhysics
{
    public static List<RaycastHit> ConeCastAll(Vector3 origin, float maxRadius, Vector3 direction, float maxDistance, float coneAngle)
    {
        RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin - direction.normalized * maxRadius, maxRadius, direction, maxDistance);
        List<RaycastHit> coneCastHits = new List<RaycastHit>();

        if (sphereCastHits.Length > 0)
        {
            for (int i = 0; i < sphereCastHits.Length; i++)
            {
                Vector3 hitPoint = sphereCastHits[i].point;
                Vector3 directionToHit = hitPoint - origin;
                float angleToHit = Vector3.Angle(direction, directionToHit);

                if (angleToHit < coneAngle)
                {
                    coneCastHits.Add(sphereCastHits[i]);
                }
            }
        }

        return coneCastHits;
    }
}