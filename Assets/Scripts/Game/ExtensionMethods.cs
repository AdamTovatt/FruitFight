using System;
using System.Collections;
using UnityEngine;

public static class ExtensionMethods
{
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
