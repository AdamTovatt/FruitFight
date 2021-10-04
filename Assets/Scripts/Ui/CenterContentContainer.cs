using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CenterContentContainer : MonoBehaviour
{
    public List<RectTransform> Content;
    public float SpaceBetween = 20f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        CenterContent();
    }

    public void CenterContent()
    {
        if (rectTransform == null)
            rectTransform = gameObject.GetComponent<RectTransform>();

        List<RectTransform> activeObjects = Content.Where(x => x.gameObject.activeSelf).ToList();

        float totalMarginSpace = SpaceBetween * (activeObjects.Count - 1);
        float totalOccupiedSpace = 0;

        foreach(RectTransform rect in activeObjects)
        {
            totalOccupiedSpace += rect.sizeDelta.x;
        }

        float currentXPosition = -1 * ((totalMarginSpace + totalOccupiedSpace) / 2);

        foreach(RectTransform rect in activeObjects)
        {
            rect.transform.localPosition = new Vector3(rectTransform.localPosition.x + currentXPosition, 0);
            currentXPosition += rect.sizeDelta.x + SpaceBetween;
        }
    }
}
