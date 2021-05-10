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

    private float lastChangeTime;
    private bool firstKeyStroke = true;

    void Awake()
    {
        _slider = gameObject.GetComponent<Slider>();
        _slider.onValueChanged.AddListener(delegate { ValueChanged(_slider.value); });
        _slider.maxValue = Choices.Count;
        _slider.minValue = -1;
        lastChangeTime = Time.time;
    }

    private void ValueChanged(float value)
    {
        if (Time.time - lastChangeTime < 0.2f)
            return;

        if (firstKeyStroke)
        {
            firstKeyStroke = false;
            OnValueChanged?.Invoke(this, Choices[1]);
            _slider.value = _slider.minValue + 2;
            lastChangeTime = Time.time;
            return;
        }

        if (value == Choices.Count)
        {
            _slider.value = _slider.minValue + 1;
        }
        else if(value == -1)
        {
            _slider.value = _slider.maxValue - 1;
        }
        else
        {
            OnValueChanged?.Invoke(this, Choices[(int)value]);
        }
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
