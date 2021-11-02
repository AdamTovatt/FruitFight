using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayerInfo : MonoBehaviour
{
    const int healthDivider = 20;

    public Image PlayerPortraitBackground;
    public Image PlayerPortrait;
    public GameObject UiHeartPrefab;

    public UiCountDisplay JellyBeanCounter;
    public UiCountDisplay CoinCounter;

    public float HeartSpacing = 60f;
    public float MarginY = 30f;
    public float MarginX = 50f;

    private RectTransform rectTransform;
    private List<UiHeart> heartSprites;

    private PlayerInformation playerInformation;
    bool isLeft;

    public void Init(PlayerInformation playerInformation, bool isLeft)
    {
        int xPivot = isLeft ? 0 : 1;

        this.playerInformation = playerInformation;
        this.isLeft = isLeft;

        int hearts = (int)(playerInformation.Health.StartHealth / healthDivider);

        rectTransform.anchorMin = new Vector2(xPivot, 1);
        rectTransform.anchorMax = new Vector2(xPivot, 1);
        rectTransform.pivot = new Vector2(xPivot, 1);
        rectTransform.anchoredPosition = new Vector3(MarginX * (isLeft ? 1 : -1), -MarginY, 0);

        PlayerPortraitBackground.rectTransform.anchorMin = new Vector2(xPivot, 1);
        PlayerPortraitBackground.rectTransform.anchorMax = new Vector2(xPivot, 1);
        PlayerPortraitBackground.rectTransform.pivot = new Vector2(xPivot, 1);
        PlayerPortraitBackground.rectTransform.localPosition = Vector3.zero;

        Texture2D image = null;
        if (!CustomNetworkManager.IsOnlineSession)
        {
            image = playerInformation.Configuration.Portrait;
        }
        else
        {
            if(isLeft)
                image = PlayerNetworkIdentity.LocalPlayerInstance.Portrait;
            else
                image = PlayerNetworkIdentity.OtherPlayerInstance.Portrait;
        }

        PlayerPortrait.sprite = Sprite.Create(image, new Rect(new Vector2(0, 0), new Vector2(image.width, image.height)), new Vector2(image.width / 2, image.height / 2));

        for (int i = 0; i < hearts; i++)
        {
            UiHeart heart = Instantiate(UiHeartPrefab, transform).GetComponent<UiHeart>();
            heart.Init(isLeft);
            RectTransform heartTransform = heart.GetComponent<RectTransform>();
            heartTransform.anchorMin = new Vector2(xPivot, 1);
            heartTransform.anchorMax = new Vector2(xPivot, 1);
            heartTransform.pivot = new Vector2(xPivot, 1);
            heartTransform.anchoredPosition = new Vector3((PlayerPortraitBackground.rectTransform.sizeDelta.x + i * HeartSpacing + (HeartSpacing / 3)) * (isLeft ? 1 : -1), -20, 0);
            heartSprites.Add(heart);
        }

        playerInformation.Health.OnHealthUpdated += UpdateHearts;
        playerInformation.Movement.OnJellyBeansUpdated += UpdateJellyBeans;
        playerInformation.Movement.OnCoinsUpdated += UpdateCoins;

        JellyBeanCounter.Initialize(!isLeft);
        CoinCounter.Initialize(!isLeft);
    }

    private void UpdateCoins(int newCount)
    {
        CoinCounter.SetCount(newCount);
    }

    private void UpdateJellyBeans(int newCount)
    {
        JellyBeanCounter.SetCount(newCount);
    }

    private void UpdateHearts()
    {
        float heartValue = playerInformation.Health.CurrentHealth / healthDivider;

        for (int i = 0; i < heartSprites.Count; i++)
        {
            heartSprites[i].SetSpriteFromValue(Mathf.Clamp(heartValue, 0, 1));
            heartValue -= 1;
        }
    }

    private void OnDestroy()
    {
        playerInformation.Health.OnHealthUpdated -= UpdateHearts;
        playerInformation.Movement.OnJellyBeansUpdated -= UpdateJellyBeans;
        playerInformation.Movement.OnCoinsUpdated -= UpdateCoins;
    }

    private void Awake()
    {
        heartSprites = new List<UiHeart>();
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateHearts();
    }
}
