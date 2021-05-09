using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceSlider : MonoBehaviour
{
    public Slider Slider { get { return _slider; } }
    private Slider _slider;

    public TextMeshProUGUI Text;
    public List<int> Choices;

    public delegate void ValueChangedHandler(MultipleChoiceSlider sender, int value);
    public event ValueChangedHandler OnValueChanged;

    void Awake()
    {
        _slider = gameObject.GetComponent<Slider>();
        _slider.onValueChanged.AddListener(delegate { ValueChanged(_slider.value); });
        _slider.maxValue = Choices.Count;
    }

    private void ValueChanged(float value)
    {
        if (value == Choices.Count)
        {
            _slider.value = _slider.minValue;
        }
        else
        {
            OnValueChanged?.Invoke(this, (int)value);
        }
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
