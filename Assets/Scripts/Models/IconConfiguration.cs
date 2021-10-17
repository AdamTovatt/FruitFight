using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IconConfiguration
{
    public List<IconConfigurationEntry> Icons;

    public bool IconsHasBeenLoaded { get { return loadedIcons != null; } }

    private static IconConfiguration loadedConfiguration;
    private static List<UiIcon> loadedIcons;
    private static Dictionary<string, UiIcon> iconDictionary;

    public static IconConfiguration Get()
    {
        if (loadedConfiguration != null)
            return loadedConfiguration;

        loadedConfiguration = JsonUtility.FromJson<IconConfiguration>(WorldUtilities.LoadTextFile("Configuration/IconConfiguration"));
        return loadedConfiguration;
    }

    public void LoadIconsFromResources()
    {
        if (loadedIcons != null)
        {
            Debug.LogError("Icons have already been loaded");
            return;
        }

        List<UiIcon> result = new List<UiIcon>();

        foreach (IconConfigurationEntry icon in Icons)
        {
            if (!icon.VaryByDevice)
            {
                Sprite sprite = Resources.Load<Sprite>(string.Format("Icons/{0}", icon.FileName));
                result.Add(new UiIcon() { ImageKeyboard = sprite, Name = icon.Name });
            }
            else
            {
                Sprite keyboardImage = Resources.Load<Sprite>(string.Format("Icons/{0}", icon.FileNameKeyboard));
                Sprite controllerImage = Resources.Load<Sprite>(string.Format("Icons/{0}", icon.FileNameController));
                result.Add(new UiIcon() { ImageController = controllerImage, ImageKeyboard = keyboardImage, VaryByDevice = true, Name = icon.Name });
            }
        }

        loadedIcons = result;
    }

    public void CreateIconDictionary()
    {
        if (iconDictionary != null)
            return;

        iconDictionary = new Dictionary<string, UiIcon>();

        if (loadedIcons == null)
            LoadIconsFromResources();

        foreach (UiIcon icon in loadedIcons)
        {
            try
            {
                iconDictionary.Add(icon.Name, icon);
            }
            catch (ArgumentException exception)
            {
                Debug.Log("Error when creating icon dictionary: " + exception.Message);
            }
        }
    }

    public UiIcon GetIcon(string name)
    {
        if (iconDictionary == null)
            CreateIconDictionary();

        if (iconDictionary.ContainsKey(name))
            return iconDictionary[name];

        throw new Exception("Icon with specified name does not exist (or at least has not been loaded correctly): " + name);
    }
}

[Serializable]
public class IconConfigurationEntry
{
    public string Name;
    public bool VaryByDevice;
    public string FileName;
    public string FileNameKeyboard;
    public string FileNameController;
}

[Serializable]
public class UiIcon
{
    public Sprite Image { get { return ImageKeyboard; } }

    public string Name;
    public bool VaryByDevice;
    public Sprite ImageKeyboard;
    public Sprite ImageController;

    public Sprite GetSpriteByDevice(Device device)
    {
        Sprite iconImage = null;

        if (device == Device.Unspecified)
            return Image;

        if (device == Device.Keyboard)
        {
            iconImage = ImageKeyboard;
            if (iconImage == null)
                iconImage = Image;
        }
        else
        {
            iconImage = ImageController;
            if (iconImage == null)
                iconImage = Image;
        }

        return iconImage;
    }
}

public enum Device
{
    Keyboard, Gamepad, Unspecified
}