using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;

public class BrowseLevelsScreen : MonoBehaviour
{
    public Button CloseButton;
    public Button UploadLevelButton;
    public RectTransform ButtonContainerSizeReference;

    public GameObject ButtonContainerPrefab;

    private LevelButtonContainer instantiatedButtonContainer;

    private static readonly HttpClient httpClient = new HttpClient();
    private static readonly string apiPath = "https://fruit-fight-api.herokuapp.com";

    private void Start()
    {
        string response = httpClient.GetAsync(apiPath + "/api/level/list").Result.Content.ReadAsStringAsync().Result;

        List<WorldMetadata> levels = JsonConvert.DeserializeObject<WorldMetadataResponse>("{\"levels\":" + response + "}").Levels;

        ShowLevels(levels);
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
