using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourMenu : MonoBehaviour
{
    public Button MoveButton;
    public Button DetailColorButton;
    public Button CloseButton;
    public TextMeshProUGUI DetailColorButtonText;

    public DetailColorMenu DetailColorMenu;

    private Block currentBlock;
    private DetailColorController currentDetailColor;

    private Color enabledColor;

    private void Start()
    {
        enabledColor = DetailColorButtonText.color;

        MoveButton.onClick.AddListener(() => { Debug.Log("move"); Move(); });
        DetailColorButton.onClick.AddListener(() => { Debug.Log("detailcolor"); DetailColor(); });
        CloseButton.onClick.AddListener(() => { WorldEditorUi.Instance.CloseBehaviourMenu(); });
    }

    private void DetailColor()
    {
        currentDetailColor = currentBlock.Instance.GetComponent<DetailColorController>();
        if (currentDetailColor == null)
            return;

        DetailColorMenu.gameObject.SetActive(true);
        DetailColorMenu.NextColorButton.Select();
        DetailColorMenu.Show(currentBlock);
        DetailColorMenu.OnClosed += (DetailColor color) => 
        {
            DetailColorPropertyCollection detailColorCollection = ((DetailColorPropertyCollection)currentBlock.BehaviourProperties.GetProperties(currentDetailColor));
            detailColorCollection.Color = color;
            detailColorCollection.ApplyValues(currentDetailColor);
            DetailColorMenu.gameObject.SetActive(false);
            MoveButton.Select();
        };
    }

    private void Move()
    {

    }

    public void Show(Block block)
    {
        currentBlock = block;
        currentDetailColor = block.Instance.GetComponent<DetailColorController>();
        
        if(currentDetailColor == null)
        {
            DetailColorButtonText.color = Color.grey;
        }
        else
        {
            DetailColorButtonText.color = enabledColor;
        }

        MoveButton.Select();
    }
}
