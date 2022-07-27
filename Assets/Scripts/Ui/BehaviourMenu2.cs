using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourMenu2 : PanelBase
{
    public BlockInspector Inspector;
    public GameObject BlockInpsectorParent;
    public Button CloseInspectorButton;

    private void Awake()
    {
        BindEvents();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        CloseInspectorButton.onClick.AddListener(() => WorldEditorUi.Instance.CloseBehaviourMenu());
    }

    private void UnBindEvents()
    {
        CloseInspectorButton.onClick.RemoveAllListeners();
    }

    public override bool Show()
    {
        Block block = WorldEditor.Instance.GetCurrentHighlightedBlock();
        if (block == null)
            return false;

        WorldEditorUi.Instance.EnterUiControlMode();
        BlockInpsectorParent.SetActive(true);
        Inspector.gameObject.SetActive(true);
        Inspector.Show(block);

        return true;
    }

    public override bool Hide()
    {
        Inspector.Clean();
        Inspector.gameObject.SetActive(false);
        WorldEditorUi.Instance.ExitUiControlMode();

        return true;
    }
}
