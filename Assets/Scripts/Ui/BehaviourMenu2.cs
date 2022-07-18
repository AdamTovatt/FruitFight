using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourMenu2 : MonoBehaviour
{
    public BlockInspector Inspector;
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
        CloseInspectorButton.onClick.AddListener(() => WorldEditorUi.Instance.CloseBehaviourMenu2());
    }

    private void UnBindEvents()
    {
        CloseInspectorButton.onClick.RemoveAllListeners();
    }

    public void Show(Block block)
    {
        Inspector.gameObject.SetActive(true);
        Inspector.Show(block);
    }

    public void Hide()
    {
        Inspector.Clean();
        Inspector.gameObject.SetActive(false);
    }
}
