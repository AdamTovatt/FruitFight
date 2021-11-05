using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiCountDisplay : MonoBehaviour
{
    public Image CounterBackground;
    public TextMeshProUGUI CounterText;
    public Image CounterIcon;
    public Wobble IconWobble;
    public Image CounterBackgroundMirrored;
    public TextMeshProUGUI CounterTextMirrored;
    public Image CounterIconMirrored;
    public Wobble IconWobbleMirrored;

    public int NormalXPosition;
    public int MirroredXPosition;

    public float SizeMValue = 44f;
    public float SizeKValue = 1f;

    private bool isMirrored;

    private RectTransform rectTransform;

    public void Initialize(bool isMirrored)
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        this.isMirrored = isMirrored;

        if(!isMirrored)
        {
            CounterIconMirrored.gameObject.SetActive(false);
            CounterTextMirrored.gameObject.SetActive(false);
            CounterBackgroundMirrored.gameObject.SetActive(false);
            rectTransform.anchoredPosition = new Vector3(NormalXPosition, rectTransform.anchoredPosition.y);
        }
        else
        {
            CounterIcon.gameObject.SetActive(false);
            CounterText.gameObject.SetActive(false);
            CounterBackground.gameObject.SetActive(false);
            rectTransform.anchoredPosition = new Vector3(MirroredXPosition, rectTransform.anchoredPosition.y);
        }
    }

    public void SetCount(int count)
    {
        if (!isMirrored)
        {
            IconWobble.StartWobble();

            CounterText.text = count.ToString();
            CounterText.ForceMeshUpdate();

            CounterBackground.rectTransform.sizeDelta = new Vector2(SizeMValue + CounterText.bounds.size.x * SizeKValue, CounterBackground.rectTransform.sizeDelta.y);
        }
        else
        {
            IconWobbleMirrored.StartWobble();

            CounterTextMirrored.text = count.ToString();
            CounterTextMirrored.ForceMeshUpdate();

            CounterBackgroundMirrored.rectTransform.sizeDelta = new Vector2(SizeMValue + CounterTextMirrored.bounds.size.x * SizeKValue, CounterBackgroundMirrored.rectTransform.sizeDelta.y);
        }
    }
}
