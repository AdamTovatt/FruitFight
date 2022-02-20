using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFootRotator : MonoBehaviour
{
    public IkFootSolver footSolver;

    private void Update()
    {
        transform.forward = footSolver.GroundForward;
    }
}
