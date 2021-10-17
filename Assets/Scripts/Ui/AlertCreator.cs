using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class AlertCreator : MonoBehaviour
{
    public GameObject AlertPrefab;
    public GameObject NotificationPrefab;
    public List<UiIcon> Icons { get; set; }

    private List<Notification> notifications = new List<Notification>();

    public static AlertCreator Instance { get; private set; }

    private void Awake()
    {
        if (Icons == null || Icons.Count == 0)
        {
            Icons = new List<UiIcon>();

            IconConfiguration iconConfiguration = IconConfiguration.Get();
            foreach (IconConfigurationEntry icon in iconConfiguration.Icons)
            {
                if (!icon.VaryByDevice)
                {
                    Sprite sprite = Resources.Load<Sprite>(string.Format("Icons/{0}", icon.FileName));
                    Icons.Add(new UiIcon() { ImageKeyboard = sprite, Name = icon.Name });
                }
                else
                {
                    Sprite keyboardImage = Resources.Load<Sprite>(string.Format("Icons/{0}", icon.FileNameKeyboard));
                    Sprite controllerImage = Resources.Load<Sprite>(string.Format("Icons/{0}", icon.FileNameController));
                    Icons.Add(new UiIcon() { ImageController = controllerImage, ImageKeyboard = keyboardImage, VaryByDevice = true, Name = icon.Name });
                }
            }
        }
    }

    public static void SetInstance(AlertCreator newInstance)
    {
        Instance = newInstance;
    }

    public Alert CreateAlert(string text, List<string> buttons)
    {
        Alert alert = Instantiate(AlertPrefab, transform).GetComponent<Alert>();
        alert.Initialize(text, buttons);
        AudioManager.Instance.Play("windowOpen");
        return alert;
    }

    public Alert CreateAlert(string text)
    {
        return CreateAlert(text, new List<string>() { "Ok" });
    }

    public List<Notification> CreateNotification(string text)
    {
        return CreateNotification(text, 5f);
    }

    public List<Notification> CreateNotification(string text, float displayTime)
    {
        return CreateNotification(text, displayTime, null);
    }

    public UiIcon GetIcon(string name)
    {
        return Icons.Where(x => x.Name == name).FirstOrDefault();
    }

    private Notification CreateNotification(string text, float displayTime, string iconName, bool keyboard)
    {
        Sprite iconImage = null;

        if (!string.IsNullOrEmpty(iconName))
        {
            UiIcon icon = GetIcon(iconName);
            if (icon != null)
            {
                iconImage = icon.GetSpriteByDevice(keyboard ? Device.Keyboard : Device.Gamepad);
            }
            else
            {
                Debug.LogError("No such icon for notification: " + iconName);
            }
        }

        Notification notification = Instantiate(NotificationPrefab, transform).GetComponent<Notification>();
        notification.Initialize(text, displayTime, iconImage);

        notifications = notifications.Where(x => x != null).ToList(); //clean notifications list
        foreach (Notification previousNotification in notifications)
            previousNotification.MoveDown(notification.Background.rectTransform.sizeDelta.y);

        notifications.Add(notification);
        return notification;
    }

    public List<Notification> CreateNotification(string text, float displayTime, string iconName)
    {
        bool keyboard = false;
        bool controller = false;

        if (iconName != null) //if we don't have an icon we don't need to find the icon for the right device
        {
            if (GameManager.Instance != null)
            {
                foreach (PlayerInformation playerConfig in GameManager.Instance.Players)
                {
                    if (playerConfig.Configuration.Input.currentControlScheme == "Gamepad")
                    {
                        controller = true;
                    }
                    else if (playerConfig.Configuration.Input.currentControlScheme == "Keyboard")
                    {
                        keyboard = true;
                    }
                    else
                    {
                        Debug.LogError("Unknown controlScheme: " + playerConfig.Configuration.Input.currentControlScheme);
                    }
                }
            }
        }

        List<Notification> createdNotifications = new List<Notification>();

        if (keyboard && controller) //both inputs at the same time
        {
            if (keyboard) //Ugh, this doesn't really feel like the best solution
            {
                createdNotifications.Add(CreateNotification(keyboard && controller ? "(keyboard) " + text : text, displayTime, iconName, true));
            }
            if (controller)
            {
                createdNotifications.Add(CreateNotification(keyboard && controller ? "(gamepad) " + text : text, displayTime, iconName, false));
            }
        }
        else
        {
            createdNotifications.Add(CreateNotification(text, displayTime, iconName, keyboard));
        }

        return createdNotifications;
    }
}
