using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateComponentWithDelay : MonoBehaviour
{
    public List<MonoBehaviour> Components;
    public float Delay;
    public List<Component> DeactivateComponents;

    private Coroutine coroutine;

    private void Awake()
    {
        foreach (MonoBehaviour component in Components)
        {
            component.enabled = false;
        }
    }

    private void Start()
    {
        coroutine = StartCoroutine(ActivateWithDelay());
    }

    private IEnumerator ActivateWithDelay()
    {
        yield return new WaitForSeconds(Delay);

        foreach (MonoBehaviour component in Components)
            component.enabled = true;

        foreach (Component component in DeactivateComponents)
            Destroy(component);

        coroutine = null;
    }

    private void OnDestroy()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
