using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorLoadLevelScreen : MonoBehaviour
{
    public RectTransform ButtonContainerSizeReference;
    public Button CloseButton;

    public GameObject ButtonContainerPrefab;

    private LevelButtonContainer instantiatedButtonContainer;

    private void Awake()
    {
        CloseButton.onClick.AddListener(OnClose);
    }

    private void OnClose()
    {
        WorldEditorUi.Instance.EnableSectionPanelsAgain(gameObject);
        if (instantiatedButtonContainer != null)
        {
            instantiatedButtonContainer.Remove();
            Destroy(instantiatedButtonContainer.gameObject);
        }
        WorldEditorUi.Instance.PauseMenu.LoadLevelButton.Select();
        gameObject.SetActive(false);
    }

    public void Close()
    {
        OnClose();
    }

    public LevelButtonContainer Show()
    {
        if(instantiatedButtonContainer != null)
        {
            instantiatedButtonContainer.Remove();
            Destroy(instantiatedButtonContainer.gameObject);
        }

        instantiatedButtonContainer = Instantiate(ButtonContainerPrefab, transform).GetComponent<LevelButtonContainer>();
        instantiatedButtonContainer.SetSize(ButtonContainerSizeReference.sizeDelta.x, ButtonContainerSizeReference.sizeDelta.y);
        instantiatedButtonContainer.SetPosition(ButtonContainerSizeReference.localPosition.x, ButtonContainerSizeReference.localPosition.y);
        instantiatedButtonContainer.DisableBackgroundImage();

        instantiatedButtonContainer.Show(FileHelper.LoadMetadatasFromDisk());

        WorldEditorUi.Instance.DisableAllButOneSectionPanels(gameObject);

        return instantiatedButtonContainer;
    }
}
