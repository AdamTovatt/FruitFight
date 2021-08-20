using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlertCreator : MonoBehaviour
{
    public GameObject AlertPrefab;
    public GameObject NotificationPrefab;
    public List<NotificationIcon> Icons;

    private List<Notification> notifications = new List<Notification>();

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

    public Notification CreateNotification(string text, float displayTime)
    {
        return CreateNotification(text, displayTime, null);
    }

    public Notification CreateNotification(string text, float displayTime, string iconName)
    {
        Sprite iconImage = null;

        if (!string.IsNullOrEmpty(iconName))
        {
            NotificationIcon icon = Icons.Where(x => x.Name == iconName).FirstOrDefault();
            if (icon != null)
            {
                iconImage = icon.Image;
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
}
