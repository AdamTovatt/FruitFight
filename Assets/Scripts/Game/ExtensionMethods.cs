using System;
using System.Collections;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Call method with delay in seconds
    /// </summary>
    /// <param name="monoBehaviour"></param>
    /// <param name="method">The method to call</param>
    /// <param name="delay">The delay IN SECONDS</param>
    public static void CallWithDelay(this MonoBehaviour monoBehaviour, Action method, float delay)
    {
        monoBehaviour.StartCoroutine(CallWithDelayRoutine(method, delay));
    }

    private static IEnumerator CallWithDelayRoutine(Action method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }
}
