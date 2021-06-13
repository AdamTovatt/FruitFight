using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartialBlockIntersection
{
    public Vector3 CornerPosition { get; set; }
    public Vector3 Size { get; set; }

    public Vector3 CenterPoint { get { return new Vector3(CornerPosition.x + Size.x / 2, CornerPosition.y - Size.y / 2, CornerPosition.z + Size.z / 2); } }
}
