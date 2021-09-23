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
    public Button ChangeViewModeButton;
    public RectTransform ButtonContainerSizeReference;
    public TextMeshProUGUI LoadingText;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI ViewModeButtonText;

    public GameObject ButtonContainerPrefab;

    public LevelDetailsScreen LevelDetailsScreen;

    private LevelButtonContainer instantiatedButtonContainer;

    private int currentPage = 0;
    private bool viewingLocalLevels;

    private void Start()
    {
        CloseButton.onClick.AddListener(MainMenuUi.Instance.ExitBrowseLevelsScreen);
        ChangeViewModeButton.onClick.AddListener(ViewModeButtonClicked);
    }

    public void Show()
    {
        ViewModeWasChanged(viewingLocalLevels);

        PopulateLevelList();
    }

    private void PopulateLevelList()
    {
        if (!viewingLocalLevels)
        {
            LoadingText.gameObject.SetActive(true);
            FetchLevels();
        }
        else
        {
            ShowLevels(FileHelper.LoadMetadatasFromDisk());
        }
    }

    private void ViewModeButtonClicked()
    {
        ViewModeWasChanged(!viewingLocalLevels);

        PopulateLevelList();
    }

    private void ViewModeWasChanged(bool newViewModeValue)
    {
        viewingLocalLevels = newViewModeValue;

        if (viewingLocalLevels)
        {
            TitleText.text = "Levels from the local library:";
            ViewModeButtonText.text = "Show online library";
        }
        else
        {
            TitleText.text = "Levels from the online library:";
            ViewModeButtonText.text = "Show local library";
        }
    }

    private async void FetchLevels()
    {
        CleanLevelList();

        WorldMetadataResponse response = await ApiLevelManager.GetLevelsList(6, currentPage);

        if (response != null)
        {
            ShowLevels(response.Levels);
        }

        LoadingText.gameObject.SetActive(false);
    }

    private void LevelWasSelected(WorldMetadata metadata)
    {
        LevelDetailsScreen.gameObject.SetActive(true);
        LevelDetailsScreen.Show(metadata, this, viewingLocalLevels);
        gameObject.SetActive(false);
    }

    private void ButtonContainerSwitchedPage(int newOffset)
    {
        Debug.Log("new offset: " + newOffset);
        FetchLevels();
    }

    private void CleanLevelList()
    {
        if (instantiatedButtonContainer != null)
        {
            Debug.Log("clean");
            instantiatedButtonContainer.OnPageSwitchRequested -= ButtonContainerSwitchedPage;
            instantiatedButtonContainer.Remove();
            Destroy(instantiatedButtonContainer.gameObject);
        }
    }

    public LevelButtonContainer ShowLevels(List<WorldMetadata> levels)
    {
        CleanLevelList();

        instantiatedButtonContainer = Instantiate(ButtonContainerPrefab, transform).GetComponent<LevelButtonContainer>();

        instantiatedButtonContainer.ClearLevelSelectSubscribersOnEventInvoke = false;

        if (!viewingLocalLevels)
            instantiatedButtonContainer.OnPageSwitchRequested += ButtonContainerSwitchedPage;

        instantiatedButtonContainer.SetSize(ButtonContainerSizeReference.sizeDelta.x, ButtonContainerSizeReference.sizeDelta.y);
        instantiatedButtonContainer.SetPosition(ButtonContainerSizeReference.localPosition.x, ButtonContainerSizeReference.localPosition.y);
        instantiatedButtonContainer.DisableBackgroundImage();

        instantiatedButtonContainer.Show(levels);

        instantiatedButtonContainer.OnLevelWasSelected += LevelWasSelected;

        return instantiatedButtonContainer;
    }
}
