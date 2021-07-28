using Assets.Scripts.Models;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailColorMenu : MonoBehaviour
{
    public Button NextColorButton;
    public Button PreviousColorButton;
    public Button CloseButton;

    public TextMeshProUGUI ColorText;

    public delegate void OnClosedHandler(DetailColor color);
    public event OnClosedHandler OnClosed;

    public DetailColor CurrentColor { get { return colors[currentColor]; } }
    private int currentColor;

    private DetailColor[] colors;

    private void Start()
    {
        colors = (DetailColor[])Enum.GetValues(typeof(DetailColor));
        CloseButton.onClick.AddListener(Close);
        NextColorButton.onClick.AddListener(NextColor);
        PreviousColorButton.onClick.AddListener(PreviousColor);
    }

    private void NextColor()
    {
        currentColor++;
        if (currentColor >= colors.Length)
            currentColor = 0;
        DisplayColorText();
    }

    private void PreviousColor()
    {
        currentColor--;
        if (currentColor < 0)
            currentColor = colors.Length - 1;

        DisplayColorText();
    }

    private void DisplayColorText()
    {
        ColorText.text = CurrentColor.ToString();
    }

    public void Show(Block block)
    {
        DetailColorPropertyCollection detailColor = (DetailColorPropertyCollection)block.BehaviourProperties.GetProperties(typeof(DetailColorController));
        currentColor = Array.IndexOf(colors, detailColor.Color);

        DisplayColorText();
    }

    public void Close()
    {
        OnClosed?.Invoke(CurrentColor);
        OnClosed = null;
    }
}
