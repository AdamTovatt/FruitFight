using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrowseLevelsScreen : MonoBehaviour
{
    public Button CloseButton;
    public Button UploadLevelButton;
    public RectTransform ButtonContainerSizeReference;
    public TextMeshProUGUI LoadingText;

    public GameObject ButtonContainerPrefab;

    private LevelButtonContainer instantiatedButtonContainer;

    private int currentPage = 0;

    private void Start()
    {
        CloseButton.onClick.AddListener(MainMenuUi.Instance.ExitBrowseLevelsScreen);
    }

    public void Show()
    {
        LoadingText.gameObject.SetActive(true);
        FetchLevels();
    }

    private async void FetchLevels()
    {
        WorldMetadataResponse response = await ApiLevelManager.GetLevelsList(6, currentPage);

        if(response != null)
        {
            ShowLevels(response.Levels);
        }

        LoadingText.gameObject.SetActive(false);
    }

    private void ButtonContainerSwitchedPage(int newOffset)
    {
        Debug.Log("new offset: " + newOffset);
        FetchLevels();
    }

    public LevelButtonContainer ShowLevels(List<WorldMetadata> levels)
    {
        if (instantiatedButtonContainer != null)
        {
            instantiatedButtonContainer.OnPageSwitchRequested -= ButtonContainerSwitchedPage;
            instantiatedButtonContainer.Remove();
            Destroy(instantiatedButtonContainer.gameObject);
        }

        instantiatedButtonContainer = Instantiate(ButtonContainerPrefab, transform).GetComponent<LevelButtonContainer>();

        instantiatedButtonContainer.OnPageSwitchRequested += ButtonContainerSwitchedPage;
        instantiatedButtonContainer.SetSize(ButtonContainerSizeReference.sizeDelta.x, ButtonContainerSizeReference.sizeDelta.y);
        instantiatedButtonContainer.SetPosition(ButtonContainerSizeReference.localPosition.x, ButtonContainerSizeReference.localPosition.y);
        instantiatedButtonContainer.DisableBackgroundImage();

        instantiatedButtonContainer.Show(levels);

        return instantiatedButtonContainer;
    }
}
