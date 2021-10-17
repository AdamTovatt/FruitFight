using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public Image Icon;
    public Image Background;
    public Tween Tween;
    public float Margin = 20f;

    public void Initialize(string text, float displayTime, Sprite icon = null)
    {
        Text.text = text;
        Text.ForceMeshUpdate();

        float iconWidth = Icon.rectTransform.sizeDelta.x;
        float textWidth = Text.textBounds.size.x;

        float backgroundHeight = Background.rectTransform.sizeDelta.y;

        if(icon != null)
        {
            Icon.sprite = icon;
            Background.rectTransform.sizeDelta = new Vector2(textWidth + iconWidth + Margin * 2, backgroundHeight + 20f);
            Icon.rectTransform.localPosition = new Vector3((textWidth / -2) - Margin/2f, Icon.rectTransform.localPosition.y);
            Text.rectTransform.localPosition = new Vector3(Margin, Text.rectTransform.localPosition.y);
        }
        else
        {
            Icon.gameObject.SetActive(false);
            Background.rectTransform.sizeDelta = new Vector2(textWidth + Margin * 2, backgroundHeight);
        }

        this.CallWithDelay(TweenOut, displayTime);
    }

    public void MoveDown(float amount)
    {
        Vector3 position = Background.rectTransform.localPosition;
        Background.rectTransform.localPosition = new Vector3(position.x, position.y - amount);
    }

    public void SetAlpha(int alpha)
    {
        Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, alpha);
    }

    private void TweenOut()
    {
        Tween.ResetValues();
        Tween.StartScale = 1;
        Tween.EndScale = 0;
        Tween.DestoryOnFinished = true;
        Tween.StartTween();
    }
}
