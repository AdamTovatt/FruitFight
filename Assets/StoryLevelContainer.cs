using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryLevelContainer : MonoBehaviour
{
    public Button NextButton;
    public Button PreviousButton;
    public GameObject LevelButtonPrefab;

    public int Rows;
    public int Columns;

    private List<World> levels;
    private ProfileSave save;

    private List<StoryLevelButton> currentButtons = new List<StoryLevelButton>();

    public void Initialize(List<World> levels, ProfileSave save)
    {
        this.levels = levels;
        this.save = save;
    }

    public void DisplayPage(int page)
    {
        foreach(StoryLevelButton button in currentButtons)
        {
            Destroy(button.gameObject);
        }
        currentButtons.Clear();

        foreach (World level in levels)
        {
            StoryLevelButton button = Instantiate(LevelButtonPrefab, new Vector3(0, 0, 0), gameObject.transform.rotation, gameObject.transform).GetComponent<StoryLevelButton>();

            Sprite buttonSprite = button.CreateScaledSpriteFromTexture(level.Metadata.GetImageDataAsTexture2d());

            button.Initialize(level.Metadata.Name, buttonSprite);
            button.Button.onClick.AddListener(() => { LevelWasClicked(level); });
            currentButtons.Add(button);
        }
    }

    private void LevelWasClicked(World level)
    {
        Debug.Log("Level clicked: " + level.Metadata.Name);
    }
}
