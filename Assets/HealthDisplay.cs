using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    public GameObject HeartPrefab;
    public CenterContentContainer HeartContainer;
    public TextMeshProUGUI Title;
    public RectTransform RectTransform;

    private float hpPerHeart;
    private Health health;
    private List<UiHeart> Hearts = new List<UiHeart>();

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void Display(Health health, float hpPerHeart = 50)
    {
        this.health = health;
        this.hpPerHeart = hpPerHeart;
        Title.text = health.HealthDisplayName;

        int hearts = Mathf.RoundToInt(health.StartHealth / hpPerHeart);

        float heartWidth = HeartPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float totalWidth = heartWidth * hearts;
        totalWidth += HeartContainer.SpaceBetween * hearts + (HeartContainer.SpaceBetween * 4);

        RectTransform.sizeDelta = new Vector2(totalWidth, RectTransform.sizeDelta.y);

        for (int i = 0; i < hearts; i++)
        {
            AddHeart();
        }

        health.OnHealthUpdated += HealthUpdated;
        health.OnDied += Died;
    }

    private void Died(Health sender, CauseOfDeath causeOfDeath)
    {
        health.OnHealthUpdated -= HealthUpdated;
        health.OnDied -= Died;
        Destroy(gameObject);
    }

    private void HealthUpdated()
    {
        float hpValue = health.CurrentHealth / hpPerHeart;

        for (int i = 0; i < Hearts.Count; i++)
        {
            Hearts[i].SetSpriteFromValue(hpValue);
            hpValue -= 1;
        }
    }

    private void AddHeart()
    {
        UiHeart heart = Instantiate(HeartPrefab, HeartContainer.transform).GetComponent<UiHeart>();
        heart.Init(true);
        HeartContainer.AddContent(heart.GetComponent<RectTransform>());
        HeartContainer.CenterContent();
        Hearts.Add(heart);
    }
}
