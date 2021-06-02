using Lookups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBlockMenu : MonoBehaviour
{
    public float Margin = 30f;

    public GameObject BlockButtonPrefab;
    public GameObject BlockButtonContainer;

    private void Start()
    {

    }

    public void SetSize(int columns, int rows, int offset)
    {
        List<BlockInfo> blocks = BlockInfoLookup.GetBlockInfoContainer().Infos;

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

        for (int i = 0; i < blocks.Count; i++)
        {
            int x = i % columns;
            int y = Mathf.FloorToInt(i / (float)columns);
            float positionX =(x * buttonSideLength) + (buttonMarginX * (x + 1));
            float positionY = (y * buttonSideLength * -1) - (buttonMarginY * (y +1));
            if (!(positionX >= buttonContainer.sizeDelta.x) && !(positionY <= buttonContainer.sizeDelta.y * -1f))
            {
                BlockButton button = Instantiate(BlockButtonPrefab, new Vector3(0, 0, 0), BlockButtonContainer.transform.rotation, BlockButtonContainer.transform).GetComponent<BlockButton>();
                button.GetComponent<RectTransform>().localPosition = new Vector3(positionX, positionY, 0);
                button.Initialize(BlockThumbnailManager.BlockThumbnails[blocks[i].Prefab], blocks[i].Prefab);
                blockButtons[x][y] = button;
            }
        }

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                BlockButton button = blockButtons[x][y];
                int upY = y - 1;
                int downY = y + 1;
                int rightX = x + 1;
                int leftX = x - 1;

                if(upY < 0)
                {

                }

                BlockButton up = blockButtons[x][upY] == null ? blockButtons[0][upY] : blockButtons[x][upY];
                BlockButton down = blockButtons[x][downY] == null ? blockButtons[0][downY] : blockButtons[x][downY];
                BlockButton right = blockButtons[rightX][y] == null ? blockButtons[rightX][0] : blockButtons[rightX][y];
                BlockButton left = blockButtons[leftX][y] == null ? blockButtons[leftX][0] : blockButtons[leftX][y];

                button.SetNavigation(up.Button, down.Button, left.Button, right.Button);
            }
        }
    }


    public void Close()
    {

    }
}
