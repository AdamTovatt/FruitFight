using Lookups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBlockMenu : MonoBehaviour
{
    public GameObject BlockButtonPrefab;
    public GameObject BlockButtonContainer;

    private void Start()
    {

    }

    public void SetSize(int newX, int newY)
    {
        List<BlockInfo> blocks = BlockInfoLookup.GetBlockInfoContainer().Infos;

        float buttonSideLength = BlockButtonPrefab.GetComponent<RectTransform>().sizeDelta.x;

        RectTransform buttonContainer = BlockButtonContainer.GetComponent<RectTransform>();
        buttonContainer.sizeDelta = new Vector2(newX, newY);
        int buttonsPerRow = Mathf.FloorToInt(buttonContainer.sizeDelta.x / (buttonSideLength * 1.1f));
        int buttonsPerColumn = Mathf.FloorToInt(buttonContainer.sizeDelta.x / (buttonSideLength * 1.1f));

        float spaceLeftOverX = buttonContainer.sizeDelta.x - (buttonSideLength * buttonsPerRow);
        float buttonMarginX = spaceLeftOverX / (buttonsPerRow + 1);
        float spaceLeftOverY = buttonContainer.sizeDelta.y - (buttonSideLength * buttonsPerRow);
        float buttonMarginY = spaceLeftOverY / (buttonsPerColumn + 1);

        for (int i = 0; i < blocks.Count; i++)
        {
            int x = i % buttonsPerRow;
            int y = Mathf.FloorToInt(i / (float)buttonsPerRow);
            float positionX =(x * buttonSideLength) + (buttonMarginX * (x + 1));
            float positionY = (y * buttonSideLength * -1) - (buttonMarginY * (y +1));
            if (!(positionX >= buttonContainer.sizeDelta.x) && !(positionY <= buttonContainer.sizeDelta.y * -1f))
            {
                BlockButton button = Instantiate(BlockButtonPrefab, new Vector3(0, 0, 0), BlockButtonContainer.transform.rotation, BlockButtonContainer.transform).GetComponent<BlockButton>();
                button.GetComponent<RectTransform>().localPosition = new Vector3(positionX, positionY, 0);
                button.Initialize(BlockThumbnailManager.BlockThumbnails[blocks[i].Prefab], blocks[i].Prefab);
            }
        }
    }

    public void Close()
    {

    }
}
