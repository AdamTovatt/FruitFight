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

    public static bool? ToNullableBool(this int value)
    {
        if (value == -1)
            return null;
        if (value == 1)
            return true;
        if (value == 0)
            return false;

        throw new System.Exception(value + " is not a nullable bool");
    }

    public static int ToInt(this bool? nullableBool)
    {
        if (nullableBool == null)
            return -1;
        if ((bool)nullableBool)
            return 1;
        else
            return 0;
    }

    public static Color SetAlpha(this Color color, float newAlpha)
    {
        return new Color(color.r, color.g, color.b, newAlpha);
    }
}
