using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
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

    public void AddContent(RectTransform transform)
    {
        if (Content == null)
            Content = new List<RectTransform>();

        Content.Add(transform);
    }

    public void RemoveContent(RectTransform transform)
    {
        if (Content == null)
            return;

        Content.Remove(transform);
    }

    public void CenterContent()
    {
        if (rectTransform == null)
            rectTransform = gameObject.GetComponent<RectTransform>();

        List<RectTransform> activeObjects = Content.Where(x => x.gameObject.activeSelf).ToList();

        float totalMarginSpace = SpaceBetween * (activeObjects.Count - 1);
        float totalOccupiedSpace = 0;

        Dictionary<RectTransform, float> sizeXDictionary = new Dictionary<RectTransform, float>();

        foreach(RectTransform rect in activeObjects)
        {
            float sizeX = GetSizeX(rect);
            sizeXDictionary.Add(rect, sizeX);
            totalOccupiedSpace += sizeX;
        }

        float currentXPosition = -1 * ((totalMarginSpace + totalOccupiedSpace) / 2);

        foreach(RectTransform rect in activeObjects)
        {
            rect.transform.localPosition = new Vector3(rectTransform.localPosition.x + currentXPosition, 0);
            currentXPosition += sizeXDictionary[rect] + SpaceBetween;
        }
    }

    private float GetSizeX(RectTransform rectTransform)
    {
        TextMeshProUGUI text = rectTransform.gameObject.GetComponent<TextMeshProUGUI>();

        if (text == null)
            return rectTransform.sizeDelta.x;

        text.ForceMeshUpdate();
        return text.textBounds.size.x;
    }
}
