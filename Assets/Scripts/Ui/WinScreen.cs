using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public TextMeshProUGUI CoinsIncreasedText;
    public TextMeshProUGUI JellyBeansIncreasedText;
    public TextMeshProUGUI XpIncreasedText;

    public AnimatedCountText CoinsText;
    public AnimatedCountText JellyBeansText;
    public AnimatedCountText XpText;

    public Button ContinueButton;

    private void Start()
    {
        ContinueButton.onClick.AddListener(Continue);
    }

    private void Continue()
    {
        GameManager.Instance.ContinueFromLevel();
        ContinueButton.enabled = false;
    }
    
    public void Show(int earnedCoins, int earnedJellyBeans, int earnedXp)
    {
        CoinsIncreasedText.gameObject.SetActive(false);
        JellyBeansIncreasedText.gameObject.SetActive(false);
        XpIncreasedText.gameObject.SetActive(false);

        CoinsText.StartAnimation(0, earnedCoins);
        JellyBeansText.StartAnimation(0, earnedJellyBeans);
        XpText.StartAnimation(0, earnedXp);

        CoinsText.OnFinishedAnimating += () => { CoinsIncreasedText.gameObject.SetActive(true); CoinsIncreasedText.text = GetIncreaseText(earnedCoins); };
        JellyBeansText.OnFinishedAnimating += () => { JellyBeansIncreasedText.gameObject.SetActive(true); JellyBeansIncreasedText.text = GetIncreaseText(earnedJellyBeans); };
        XpText.OnFinishedAnimating += () => { XpIncreasedText.gameObject.SetActive(true); XpIncreasedText.text = GetIncreaseText(earnedXp); };

        ContinueButton.Select();
    }

    private string GetIncreaseText(int value)
    {
        return string.Format("(+{0})", value);
    }
}
