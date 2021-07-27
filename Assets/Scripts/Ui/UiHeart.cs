using UnityEngine;
using UnityEngine.UI;

public class UiHeart : MonoBehaviour
{
    public Sprite Full;
    public Sprite Empty;
    public Sprite HalfRight;
    public Sprite HalfLeft;

    private bool isLeft;
    private Image image;

    public void Init(bool isLeft)
    {
        image = gameObject.GetComponent<Image>();
        this.isLeft = isLeft;
    }

    public void SetSpriteFromValue(float value)
    {
        if (value > 0.5f)
        {
            image.sprite = Full;
        }
        else if (value > 0)
        {
            if (isLeft)
            {
                image.sprite = HalfLeft;
            }
            else
            {
                image.sprite = HalfRight;
            }
        }
        else
        {
            image.sprite = Empty;
        }
    }

}
