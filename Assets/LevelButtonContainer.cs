using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonContainer : MonoBehaviour
{
    public float ButtonMarginRight = 10f;
    public float ButtonMarginDown = 10f;
    public float NextPageButtonsMargin = 20f;

    public RectTransform PanelTransform;
    public GameObject LevelButtonPrefab;
    public Sprite NoLevelThumbnailDefaultImage;

    public Button NextPageButton;
    public Button PreviousPageButton;
    public TextMeshProUGUI PageNumberText;
    public Image BackgroundImage;

    public delegate void LevelWasSelected(WorldMetadata worldMetadata);
    public LevelWasSelected OnLevelWasSelected;

    private List<WorldMetadata> currentLevels;
    private List<LevelButton> currentButtons;
    private float buttonWidth;
    private float buttonHeight;
    private int currentButtonsPerPage;
    private int currentButtonOffset;

    private void Awake()
    {
        SetButtonSizeValues();
    }

    private void Start()
    {
        NextPageButton.onClick.AddListener(NextPage);
        PreviousPageButton.onClick.AddListener(PreviousPage);
    }

    private void ButtonWasClicked(WorldMetadata level)
    {
        Debug.Log("level selected: " + level.Name);
        OnLevelWasSelected?.Invoke(level);
        OnLevelWasSelected = null;
    }

    private void SetButtonSizeValues()
    {
        if (buttonWidth == 0 && buttonHeight == 0)
        {
            buttonWidth = LevelButtonPrefab.GetComponent<RectTransform>().sizeDelta.x;
            buttonHeight = LevelButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }
    }

    private void NextPage()
    {
        if (currentButtonOffset + currentButtonsPerPage >= currentLevels.Count)
            return;

        currentButtonOffset += currentButtonsPerPage;

        Show(currentLevels, false);
    }

    private void PreviousPage()
    {
        if (currentButtonOffset - currentButtonsPerPage < 0)
            currentButtonOffset = 0;
        else
            currentButtonOffset -= currentButtonsPerPage;

        Show(currentLevels, false);
    }

    public void Show(List<WorldMetadata> levels, bool selectFirstButton = true)
    {
        currentLevels = levels;

        SetButtonSizeValues();

        FillCurrentSizeWithButtons(currentButtonOffset);

        if (selectFirstButton)
        {
            if (currentButtons.Count > 0)
                currentButtons[0].Button.Select();
        }
    }

    public void SetSize(float width, float height)
    {
        PanelTransform.sizeDelta = new Vector2(width, height);
    }

    public void SetPosition(float x, float y)
    {
        PanelTransform.localPosition = new Vector3(x, y, 0);
    }

    public void DisableBackgroundImage()
    {
        BackgroundImage.enabled = false;
    }

    public void EnableBackgroundImage()
    {
        BackgroundImage.enabled = true;
    }

    public void Remove()
    {
        if (currentButtons != null)
        {
            foreach (LevelButton button in currentButtons)
            {
                button.Button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }

            currentButtons.Clear();
        }
    }

    private void FillCurrentSizeWithButtons(int buttonOffset)
    {
        if (currentButtons != null)
        {
            foreach (LevelButton button in currentButtons)
            {
                button.Button.onClick.RemoveAllListeners();
                Destroy(button.gameObject);
            }

            currentButtons.Clear();
        }
        else
        {
            currentButtons = new List<LevelButton>();
        }

        float usableSpaceY = (PanelTransform.sizeDelta.y - (NextPageButtonsMargin * 2));

        int columns = Mathf.FloorToInt(PanelTransform.sizeDelta.x / (buttonWidth + ButtonMarginRight));
        int rows = Mathf.FloorToInt(usableSpaceY / (buttonHeight + ButtonMarginDown));

        currentButtonsPerPage = columns * rows;

        float pageButtonsCenterOffset = PanelTransform.sizeDelta.x / 4f;
        NextPageButton.GetComponent<RectTransform>().localPosition = new Vector3((PanelTransform.sizeDelta.x / 2f) + pageButtonsCenterOffset, NextPageButtonsMargin, 0);
        PreviousPageButton.GetComponent<RectTransform>().localPosition = new Vector3((PanelTransform.sizeDelta.x / 2f) - pageButtonsCenterOffset, NextPageButtonsMargin, 0);
        PageNumberText.GetComponent<RectTransform>().localPosition = new Vector3((PanelTransform.sizeDelta.x / 2f), PanelTransform.sizeDelta.x < (NextPageButton.GetComponent<RectTransform>().sizeDelta.x + PreviousPageButton.GetComponent<RectTransform>().sizeDelta.x + PageNumberText.rectTransform.sizeDelta.x + ButtonMarginRight * 2) ? NextPageButtonsMargin * 2 : NextPageButtonsMargin, 0);
        PageNumberText.text = string.Format("Page: {0}/{1}", Mathf.FloorToInt((float)currentButtonOffset / currentButtonsPerPage), Mathf.FloorToInt((float)currentLevels.Count / currentButtonsPerPage));

        float edgeMarginX = (PanelTransform.sizeDelta.x - (columns * (buttonWidth + ButtonMarginRight))) / 2f;
        float edgeMarginY = Mathf.Min(ButtonMarginDown * 2, (usableSpaceY - (rows * (buttonHeight + ButtonMarginDown))) / 2f);

        int currentX = 0;
        int currentY = 0;

        foreach (WorldMetadata level in currentLevels.Skip(buttonOffset).Take(currentButtonsPerPage))
        {
            LevelButton button = Instantiate(LevelButtonPrefab, new Vector3(0, 0, 0), PanelTransform.rotation, PanelTransform).GetComponent<LevelButton>();
            button.Init(level.Name, "level text", NoLevelThumbnailDefaultImage);
            button.GetComponent<RectTransform>().localPosition = new Vector3(edgeMarginX + currentX * (buttonWidth + ButtonMarginRight), PanelTransform.sizeDelta.y - (edgeMarginY + currentY * (buttonHeight + ButtonMarginDown)), 0);
            button.Button.onClick.AddListener(() => { ButtonWasClicked(level); });
            currentButtons.Add(button);

            if (currentX < columns - 1)
            {
                currentX++;
            }
            else
            {
                currentX = 0;
                currentY++;
            }
        }
    }
}
