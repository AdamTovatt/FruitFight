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

    private void Start()
    {
        LoadingText.gameObject.SetActive(true);
        FetchLevels();
    }

    private async void FetchLevels()
    {
        WorldMetadataResponse response = await ApiLevelManager.GetLevelsList(6, 0);

        if(response != null)
        {
            ShowLevels(response.Levels);
        }

        LoadingText.gameObject.SetActive(false);
    }

    public LevelButtonContainer ShowLevels(List<WorldMetadata> levels)
    {
        if (instantiatedButtonContainer != null)
        {
            instantiatedButtonContainer.Remove();
            Destroy(instantiatedButtonContainer.gameObject);
        }

        instantiatedButtonContainer = Instantiate(ButtonContainerPrefab, transform).GetComponent<LevelButtonContainer>();
        instantiatedButtonContainer.SetSize(ButtonContainerSizeReference.sizeDelta.x, ButtonContainerSizeReference.sizeDelta.y);
        instantiatedButtonContainer.SetPosition(ButtonContainerSizeReference.localPosition.x, ButtonContainerSizeReference.localPosition.y);
        instantiatedButtonContainer.DisableBackgroundImage();

        instantiatedButtonContainer.Show(levels);

        return instantiatedButtonContainer;
    }
}
