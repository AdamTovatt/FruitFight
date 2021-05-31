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

    public void Open()
    {
        List<BlockInfo> blocks = BlockInfoLookup.GetBlockInfoContainer().Infos;

        float sizeMultiplier = WorldEditorUi.Instance.GetComponent<Canvas>().transform.localScale.magnitude;
        float buttonWidth = BlockButtonPrefab.GetComponent<RectTransform>().sizeDelta.x;

        RectTransform buttonContainer = BlockButtonContainer.GetComponent<RectTransform>();
        int buttonsPerRow = Mathf.FloorToInt(buttonContainer.sizeDelta.x / (buttonWidth * 1.1f));
        float spaceLeftOver = buttonContainer.sizeDelta.x - (buttonWidth * buttonsPerRow);
        float buttonMargin = spaceLeftOver / (buttonsPerRow + 1) * sizeMultiplier;

        Debug.Log("Button width: " + buttonWidth);
        Debug.Log("Buttons per row: " + buttonsPerRow);
        Debug.Log("Space left over: " + spaceLeftOver);
        Debug.Log("Button margin: " + buttonMargin);
        Debug.Log("Size multiplier: " + sizeMultiplier);

        for (int i = 0; i < blocks.Count; i++)
        {
            int x = i % buttonsPerRow;
            int y = Mathf.FloorToInt(i / (float)buttonsPerRow);
            float positionX = buttonContainer.position.x + (x * (buttonWidth / 1.73f) * sizeMultiplier) + buttonMargin;
            float positionY = BlockButtonContainer.transform.position.y + (y * buttonWidth * sizeMultiplier * -1) - buttonMargin;
            BlockButton button = Instantiate(BlockButtonPrefab, new Vector3(positionX, positionY, 0), BlockButtonContainer.transform.rotation, BlockButtonContainer.transform).GetComponent<BlockButton>();
            button.Initialize(BlockThumbnailManager.BlockThumbnails[blocks[i].Prefab], blocks[i].Prefab);
        }
    }

    public void Close()
    {

    }
}
