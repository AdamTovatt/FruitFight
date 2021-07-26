using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayerInfo : MonoBehaviour
{
    public Image PlayerPortraitBackground;
    public Image PlayerPortrait;
    public GameObject UiHeartPrefab;

    public float HeartSpacing = 60f;
    public float MarginY = 30f;
    public float MarginX = 50f;

    private RectTransform rectTransform;
    private List<UiHeart> hearts;

    private PlayerInformation playerInformation;
    bool isLeft;

    public void Init(PlayerInformation playerInformation, bool isLeft)
    {
        int xPivot = isLeft ? 0 : 1;

        this.playerInformation = playerInformation;
        this.isLeft = isLeft;

        int hearts = ((int)playerInformation.Health.StartHealth / 10) / 2;

        rectTransform.anchorMin = new Vector2(xPivot, 1);
        rectTransform.anchorMax = new Vector2(xPivot, 1);
        rectTransform.pivot = new Vector2(xPivot, 1);
        rectTransform.anchoredPosition = new Vector3(MarginX * (isLeft ? 1 : -1), -MarginY, 0);

        PlayerPortraitBackground.rectTransform.anchorMin = new Vector2(xPivot, 1);
        PlayerPortraitBackground.rectTransform.anchorMax = new Vector2(xPivot, 1);
        PlayerPortraitBackground.rectTransform.pivot = new Vector2(xPivot, 1);
        PlayerPortraitBackground.rectTransform.localPosition = Vector3.zero;

        for (int i = 0; i < hearts; i++)
        {
            UiHeart heart = Instantiate(UiHeartPrefab, transform).GetComponent<UiHeart>();
            heart.Init(isLeft);
            RectTransform heartTransform = heart.GetComponent<RectTransform>();
            heartTransform.anchorMin = new Vector2(xPivot, 1);
            heartTransform.anchorMax = new Vector2(xPivot, 1);
            heartTransform.pivot = new Vector2(xPivot, 1);
            heartTransform.anchoredPosition = new Vector3((PlayerPortraitBackground.rectTransform.sizeDelta.x + i * HeartSpacing + (HeartSpacing / 3)) * (isLeft ? 1 : -1), -20, 0);
        }
    }

    private void Awake()
    {
        hearts = new List<UiHeart>();
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
    }
}
