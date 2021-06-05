using Lookups;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorBlockMenu : MonoBehaviour
{
    public float Margin = 30f;
    public int DefaultColumns = 3;
    public int DefaultRows = 3;

    public GameObject BlockButtonPrefab;
    public GameObject BlockButtonContainer;

    public bool IsOpen { get; private set; }
    public int CurrentOffset { get { return selectedIndex; } }

    private List<BlockButton> currentButtons = new List<BlockButton>();
    private List<BlockInfo> currentBlockInfos = new List<BlockInfo>();
    private BlockButton[][] currentBlockButtonArray;

    private int selectedIndex;
    private BlockButton selectedButton;

    public BlockInfo CurrentBuildingBlock { get { if (selectedButton == null) return null; return selectedButton.BlockInfo; } }

    public void ThumbnailsWereCreated()
    {
        currentBlockInfos = BlockInfoLookup.GetBlockInfoContainer().Infos.Where(b => b.ShowInEditor).ToList();

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

    public void ButtonWasSelected(BlockButton button)
    {
        selectedButton = button;
        selectedIndex = button.MenuIndex;
        WorldEditor.Instance.SelectedBlock = CurrentBuildingBlock.Id;
        Debug.Log(button.BlockInfo.Prefab + " was selected");
    }

    public void Close()
    {
        SetSize(1, 1, CurrentOffset);
        IsOpen = false;
    }

    public void Open()
    {
        int items = (DefaultRows * DefaultColumns);
        SetSize(DefaultRows, DefaultColumns, (CurrentOffset / items) * items);
        Debug.Log("Set size with offset: " + (CurrentOffset / items) * items);
        IsOpen = true;
    }

    public void MoveBlockButtonSelection(Vector2Int moveVector)
    {
        if (!IsOpen)
            Open();

        bool moved = false;

        if(moveVector.x == 1)
        {
            Selectable selectable = selectedButton.Button.navigation.selectOnRight;

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

            if (selectable != null)
            {
                selectable.Select();
                ButtonWasSelected(selectable.gameObject.GetComponent<BlockButton>());
                moved = true;
            }
        }
        else if(moveVector.y == -1)
        {
            Selectable selectable = selectedButton.Button.navigation.selectOnDown;

            if (selectable != null)
            {
                selectable.Select();
                ButtonWasSelected(selectable.gameObject.GetComponent<BlockButton>());
                moved = true;
            }
        }
        else if(moveVector.y == 1)
        {
            Selectable selectable = selectedButton.Button.navigation.selectOnUp;

            if (selectable != null)
            {
                selectable.Select();
                ButtonWasSelected(selectable.gameObject.GetComponent<BlockButton>());
                moved = true;
            }
        }

        if(!moved)
        {
            selectedButton.GetComponent<Selectable>().Select();
        }
    }

    public void SetSize(int columns, int rows, int offset)
    {
        float buttonSideLength = BlockButtonPrefab.GetComponent<RectTransform>().sizeDelta.x;

        RectTransform buttonContainer = BlockButtonContainer.GetComponent<RectTransform>();
        buttonContainer.sizeDelta = new Vector2(columns * buttonSideLength + (columns + 1) * Margin, rows * buttonSideLength + (rows + 1) * Margin);

        float spaceLeftOverX = buttonContainer.sizeDelta.x - (buttonSideLength * columns);
        float buttonMarginX = spaceLeftOverX / (columns + 1);
        float spaceLeftOverY = buttonContainer.sizeDelta.y - (buttonSideLength * columns);
        float buttonMarginY = spaceLeftOverY / (rows + 1);

        BlockButton[][] blockButtons = new BlockButton[columns][];
        for (int i = 0; i < columns; i++)
        {
            blockButtons[i] = new BlockButton[rows];
        }

        currentBlockButtonArray = blockButtons;

        if(currentButtons != null)
        {
            for (int i = 0; i < currentButtons.Count; i++)
            {
                Destroy(currentButtons[i].gameObject);
            }

            currentButtons.Clear();
        }

        List<BlockInfo> limitedList = currentBlockInfos.Skip(offset).ToList();
        for (int i = 0; i < limitedList.Count; i++)
        {
            int x = i % columns;
            int y = Mathf.FloorToInt(i / (float)columns);
            float positionX =(x * buttonSideLength) + (buttonMarginX * (x + 1));
            float positionY = (y * buttonSideLength * -1) - (buttonMarginY * (y +1));
            if (!(positionX >= buttonContainer.sizeDelta.x) && !(positionY <= buttonContainer.sizeDelta.y * -1f))
            {
                BlockButton button = Instantiate(BlockButtonPrefab, new Vector3(0, 0, 0), BlockButtonContainer.transform.rotation, BlockButtonContainer.transform).GetComponent<BlockButton>();
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

                    button.SetNavigation(up, down, left, right);
                }
            }
        }
    }
}
