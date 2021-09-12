using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        List<WorldMetadata> worldMetadatas = new List<WorldMetadata>();

        string mapDirectory = string.Format("{0}/maps", Application.persistentDataPath);

        if (!Directory.Exists(mapDirectory))
            Directory.CreateDirectory(mapDirectory);

        foreach (string file in Directory.GetFiles(mapDirectory).Where(x => x.EndsWith(".meta")).ToList())
        {
            worldMetadatas.Add(WorldMetadata.FromJson(File.ReadAllText(file)));
        }

        Show(worldMetadatas);
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

        Show(currentLevels);
    }

    private void PreviousPage()
    {
        if (currentButtonOffset - currentButtonsPerPage < 0)
            currentButtonOffset = 0;
        else
            currentButtonOffset -= currentButtonsPerPage;

        Show(currentLevels);
    }

    public void Show(List<WorldMetadata> levels)
    {
        currentLevels = levels;

        SetButtonSizeValues();

        FillCurrentSizeWithButtons(currentButtonOffset);
    }

    private void FillCurrentSizeWithButtons(int buttonOffset)
    {
        if (currentButtons != null)
        {
            foreach (LevelButton button in currentButtons)
            {
                Destroy(button.gameObject);
            }

            currentButtons.Clear();
        }
        else
        {
            currentButtons = new List<LevelButton>();
        }

        float pageButtonsCenterOffset = PanelTransform.sizeDelta.x / 4f;
        NextPageButton.GetComponent<RectTransform>().localPosition = new Vector3((PanelTransform.sizeDelta.x / 2f) + pageButtonsCenterOffset, NextPageButtonsMargin, 0);
        PreviousPageButton.GetComponent<RectTransform>().localPosition = new Vector3((PanelTransform.sizeDelta.x / 2f) - pageButtonsCenterOffset, NextPageButtonsMargin, 0);

        float usableSpaceY = (PanelTransform.sizeDelta.y - (NextPageButtonsMargin * 2));

        int columns = Mathf.FloorToInt(PanelTransform.sizeDelta.x / (buttonWidth + ButtonMarginRight));
        int rows = Mathf.FloorToInt(usableSpaceY / (buttonHeight + ButtonMarginDown));

        currentButtonsPerPage = columns * rows;

        float edgeMarginX = (PanelTransform.sizeDelta.x - (columns * (buttonWidth + ButtonMarginRight))) / 2f;
        float edgeMarginY = Mathf.Min(ButtonMarginDown * 2, (usableSpaceY - (rows * (buttonHeight + ButtonMarginDown))) / 2f);

        int currentX = 0;
        int currentY = 0;

        foreach (WorldMetadata level in currentLevels.Skip(buttonOffset).Take(currentButtonsPerPage))
        {
            LevelButton button = Instantiate(LevelButtonPrefab, new Vector3(0, 0, 0), PanelTransform.rotation, PanelTransform).GetComponent<LevelButton>();
            button.Init(level.Name, "level text", NoLevelThumbnailDefaultImage);
            button.GetComponent<RectTransform>().localPosition = new Vector3(edgeMarginX + currentX * (buttonWidth + ButtonMarginRight), PanelTransform.sizeDelta.y - (edgeMarginY + currentY * (buttonHeight + ButtonMarginDown)), 0);
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
