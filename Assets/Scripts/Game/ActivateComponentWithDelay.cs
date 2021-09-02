using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateComponentWithDelay : MonoBehaviour
{
    public List<MonoBehaviour> Components;
    public float Delay;
    public List<Component> DestoryComponents;
    public List<Collider> ActivateColliders;

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

        foreach (Component component in DestoryComponents)
            Destroy(component);

        foreach (Collider collider in ActivateColliders)
            collider.enabled = true;

        coroutine = null;
    }

    private void OnDestroy()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
