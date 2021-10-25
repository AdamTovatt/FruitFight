using Lookups;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorBlockMenu : MonoBehaviour
{
    public float Margin = 30f;
    public int DefaultColumns = 3;
    public int DefaultRows = 3;
    public float ExtraMarginBottom = 50f;

    public GameObject BlockButtonPrefab;
    public GameObject BlockButtonContainer;
    public TextMeshProUGUI PageText;
    public TextMeshProUGUI NextPageInstuctionLabel;
    public TextMeshProUGUI PreviousPageInstuctionLabel;
    public Button NextPageButton;
    public Button PreviousPageButton;

    public Button DeselectButtonRight;
    public Button DeselectButtonLeft;
    public Button DeselectButtonDown;
    public Button DeselectButtonUp;

    public bool IsOpen { get; private set; }
    public int CurrentOffset { get { return selectedIndex; } }

    private List<BlockButton> currentButtons = new List<BlockButton>();
    private List<BlockInfo> currentBlockInfos = new List<BlockInfo>();
    private BlockButton[][] currentBlockButtonArray;

    private int currentPage;
    private int selectedIndex;
    private BlockButton selectedButton;

    public BlockInfo CurrentBuildingBlock { get { if (selectedButton == null) return null; return selectedButton.BlockInfo; } }

    private bool deselect = true;

    private void Start()
    {
        NextPageButton.onClick.AddListener(NextPage);
        PreviousPageButton.onClick.AddListener(PreviousPage);
        DeselectButtonRight.onClick.AddListener(Close);
        DeselectButtonDown.onClick.AddListener(Close);
        DeselectButtonRight.GetComponent<UiSelectable>().OnSelected += Deselect;
        DeselectButtonLeft.GetComponent<UiSelectable>().OnSelected += Deselect;
        DeselectButtonDown.GetComponent<UiSelectable>().OnSelected += Deselect;
        DeselectButtonUp.GetComponent<UiSelectable>().OnSelected += Deselect;
    }

    public void ThumbnailsWereCreated()
    {
        currentBlockInfos = BlockInfoLookup.GetBlockInfoContainer().Infos
            .Where(b => b.ShowInEditor)
            .OrderBy(x => x.BlockType)
            .ThenBy(x => x.Name)
            .ToList();

        Close();

        if (currentBlockInfos != null)
        {
            selectedButton = currentButtons[0];
            selectedIndex = selectedButton.MenuIndex;
        }
        else
        {
            Debug.LogError("CurrentBlockInfos is null");
        }
    }

    private void Deselect()
    {
        deselect = true;
    }

    public void ButtonWasSelected(BlockButton button)
    {
        if (button == null)
            return;

        selectedButton = button;
        selectedIndex = button.MenuIndex;
        WorldEditor.Instance.SelectedBlock = CurrentBuildingBlock.Id;

        if (!WorldEditorUi.Instance.PauseMenuIsOpen && !IsOpen && deselect)
        {
            Open();
        }
    }

    public void DisableDeselectButtons()
    {
        DeselectButtonDown.gameObject.SetActive(false);
        DeselectButtonLeft.gameObject.SetActive(false);
        DeselectButtonRight.gameObject.SetActive(false);
        DeselectButtonUp.gameObject.SetActive(false);
    }

    public void EnableDeslectButtons()
    {
        DeselectButtonDown.gameObject.SetActive(true);
        DeselectButtonLeft.gameObject.SetActive(true);
        DeselectButtonRight.gameObject.SetActive(true);
        DeselectButtonUp.gameObject.SetActive(true);
    }

    public void Close()
    {
        SetSize(1, 1, currentPage);
        IsOpen = false;
        PageText.gameObject.SetActive(false);
        NextPageInstuctionLabel.gameObject.SetActive(false);
        PreviousPageInstuctionLabel.gameObject.SetActive(false);
        deselect = false;
    }

    public void Open()
    {
        PageText.gameObject.SetActive(true);
        NextPageInstuctionLabel.gameObject.SetActive(true);
        PreviousPageInstuctionLabel.gameObject.SetActive(true);
        SetSize(DefaultRows, DefaultColumns, currentPage);
        IsOpen = true;
        SetPageText();
    }


    public void NextPage()
    {
        if (currentPage >= Mathf.FloorToInt(currentBlockInfos.Count / (DefaultColumns * DefaultRows)))
            return;

        currentPage++;
        Open();

        if (currentButtons.Count - 1 >= selectedIndex)
        {
            ButtonWasSelected(currentButtons[selectedIndex]);
        }
        else
        {
            ButtonWasSelected(currentButtons[currentButtons.Count - 1]);
        }

        SetPageText();
    }

    public void PreviousPage()
    {
        if (currentPage == 0)
            return;

        currentPage--;
        Open();

        if (currentButtons.Count - 1 >= selectedIndex)
        {
            ButtonWasSelected(currentButtons[selectedIndex]);
        }
        else
        {
            ButtonWasSelected(currentButtons[currentButtons.Count - 1]);
        }

        SetPageText();
    }

    private void SetPageText()
    {
        PageText.text = string.Format("Page {0}/{1}", (currentPage + 1).ToString(), Mathf.FloorToInt(currentBlockInfos.Count / (DefaultColumns * DefaultRows)) + 1);
    }

    public void MoveBlockButtonSelection(Vector2Int moveVector)
    {
        if (!IsOpen)
            Open();

        Selectable newSelectable = null;

        bool moved = false;

        if (moveVector.x == 1)
        {
            Selectable selectable = selectedButton.Button.navigation.selectOnRight;
            newSelectable = selectable;

            if (selectable != null)
            {
                selectable.Select();
                ButtonWasSelected(selectable.gameObject.GetComponent<BlockButton>());
                moved = true;
            }
        }
        else if (moveVector.x == -1)
        {
            Selectable selectable = selectedButton.Button.navigation.selectOnLeft;
            newSelectable = selectable;

            if (selectable != null)
            {
                selectable.Select();
                ButtonWasSelected(selectable.gameObject.GetComponent<BlockButton>());
                moved = true;
            }
        }
        else if (moveVector.y == -1)
        {
            Selectable selectable = selectedButton.Button.navigation.selectOnDown;
            newSelectable = selectable;

            if (selectable != null)
            {
                selectable.Select();
                ButtonWasSelected(selectable.gameObject.GetComponent<BlockButton>());
                moved = true;
            }
        }
        else if (moveVector.y == 1)
        {
            Selectable selectable = selectedButton.Button.navigation.selectOnUp;
            newSelectable = selectable;

            if (selectable != null)
            {
                selectable.Select();
                ButtonWasSelected(selectable.gameObject.GetComponent<BlockButton>());
                moved = true;
            }
        }

        if (!moved)
        {
            selectedButton.GetComponent<Selectable>().Select();
        }

        if (newSelectable != null)
        {
            UiSelectable uiSelectable = newSelectable.transform.GetComponent<UiSelectable>();
            if (uiSelectable != null)
                WorldEditorUi.Instance.MouseOverSelectableChecker.SetSelectedItem(uiSelectable);
        }
    }

    private void BlockButtonWasClicked()
    {
        Close();
    }

    public void SetSize(int columns, int rows, int page)
    {
        float appliedExtraMargin = ExtraMarginBottom;
        if (columns == 1 && rows == 1)
            appliedExtraMargin = 0;

        float buttonSideLength = BlockButtonPrefab.GetComponent<RectTransform>().sizeDelta.x;

        RectTransform buttonContainer = BlockButtonContainer.GetComponent<RectTransform>();
        buttonContainer.sizeDelta = new Vector2(columns * buttonSideLength + (columns + 1) * Margin, rows * buttonSideLength + (rows + 1) * Margin + appliedExtraMargin);

        float spaceLeftOverX = buttonContainer.sizeDelta.x - (buttonSideLength * columns);
        float buttonMarginX = spaceLeftOverX / (columns + 1);
        float spaceLeftOverY = buttonContainer.sizeDelta.y - appliedExtraMargin / 2 - (buttonSideLength * columns);
        float buttonMarginY = spaceLeftOverY / (rows + 1);

        BlockButton[][] blockButtons = new BlockButton[columns][];
        for (int i = 0; i < columns; i++)
        {
            blockButtons[i] = new BlockButton[rows];
        }

        currentBlockButtonArray = blockButtons;

        if (currentButtons != null)
        {
            for (int i = 0; i < currentButtons.Count; i++)
            {
                Destroy(currentButtons[i].gameObject);
            }

            currentButtons.Clear();
        }

        List<BlockInfo> limitedList = currentBlockInfos.Skip(page * (DefaultColumns * DefaultRows)).Take(DefaultRows * DefaultColumns).ToList();

        if (columns == 1 && rows == 1)
            limitedList = limitedList.Skip(selectedIndex).Take(1).ToList();

        for (int i = 0; i < limitedList.Count; i++)
        {
            int x = i % columns;
            int y = Mathf.FloorToInt(i / (float)columns);
            float positionX = (x * buttonSideLength) + (buttonMarginX * (x + 1));
            float positionY = (y * buttonSideLength * -1) - (buttonMarginY * (y + 1)) + appliedExtraMargin / 8;
            if (!(positionX >= buttonContainer.sizeDelta.x) && !(positionY <= buttonContainer.sizeDelta.y * -1f))
            {
                BlockButton button = Instantiate(BlockButtonPrefab, new Vector3(0, 0, 0), BlockButtonContainer.transform.rotation, BlockButtonContainer.transform).GetComponent<BlockButton>();
                button.transform.name = string.Format("{0} Button", limitedList[i].Name);
                button.Button.onClick.AddListener(BlockButtonWasClicked);
                button.GetComponent<RectTransform>().localPosition = new Vector3(positionX, positionY, 0);
                button.Initialize(BlockThumbnailManager.BlockThumbnails[limitedList[i].Prefab], limitedList[i].Name);
                button.BlockInfo = limitedList[i];
                button.MenuIndex = i;
                blockButtons[x][y] = button;
                currentButtons.Add(button);

                if (i == selectedIndex)
                    selectedButton = button;
            }
        }

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                BlockButton button = blockButtons[x][y];

                if (button != null)
                {
                    int upY = y - 1;
                    int downY = y + 1;
                    int rightX = x + 1;
                    int leftX = x - 1;

                    Button up = null;
                    Button down = null;
                    Button right = null;
                    Button left = null;

                    if (upY >= 0)
                    {
                        BlockButton upButton = blockButtons[x][upY] == null ? blockButtons[0][upY] : blockButtons[x][upY];
                        if (upButton != null)
                            up = upButton.Button;
                    }

                    if (downY < rows)
                    {
                        BlockButton downButton = blockButtons[x][downY] == null ? blockButtons[0][downY] : blockButtons[x][downY];
                        if (downButton != null)
                            down = downButton.Button;
                    }

                    if (rightX < columns)
                    {
                        BlockButton rightButton = blockButtons[rightX][y] == null ? blockButtons[rightX][0] : blockButtons[rightX][y];
                        if (rightButton != null)
                            right = rightButton.Button;
                    }

                    if (leftX >= 0)
                    {
                        BlockButton leftButton = blockButtons[leftX][y] == null ? blockButtons[leftX][0] : blockButtons[leftX][y];
                        if (leftButton != null)
                            left = leftButton.Button;
                    }

                    if (down == null) //setting navigation to page arrows
                    {
                        Navigation navigation = new Navigation();
                        navigation.mode = Navigation.Mode.Explicit;
                        navigation.selectOnUp = button.Button;
                        navigation.selectOnRight = null;
                        navigation.selectOnDown = null;

                        if (right == null)
                        {
                            down = NextPageButton;
                            navigation.selectOnLeft = PreviousPageButton;
                            NextPageButton.navigation = navigation;
                        }

                        if (left == null)
                        {
                            down = PreviousPageButton;
                            navigation.selectOnRight = NextPageButton;
                            PreviousPageButton.navigation = navigation;
                        }
                    }

                    button.SetNavigation(up, down, left, right);
                    button.OnSelected += ButtonWasSelected;
                }
            }
        }
    }
}
