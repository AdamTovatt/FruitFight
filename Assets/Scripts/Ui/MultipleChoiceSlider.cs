using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultipleChoiceSlider : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public List<int> Choices;

    public delegate void ValueChangedHandler(MultipleChoiceSlider sender, int value);
    public event ValueChangedHandler OnValueChanged;

    public bool InputEnabled { get; set; } = true;

    public int Value { get { return sliderValue; } }

    private int sliderValue;
    private float lastInputTime;
    private PlayerInput playerInput;
    private PlayerControls playerControls;

    void Awake()
    {
        lastInputTime = Time.time;
        playerControls = new PlayerControls();
    }

    public void InitializeInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        this.playerInput.onActionTriggered += HandleInput;
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (Time.time - lastInputTime < 0.2f)
            return;

        if (context.action.id == playerControls.Ui.Move.id)
        {
            lastInputTime = Time.time;

            Vector2 inputValue = context.ReadValue<Vector2>();

            sliderValue += inputValue.x > 0 ? 1 : -1;
            ValueChanged();
        }
    }

    private void ValueChanged()
    {
        if (sliderValue < 0)
        {
            sliderValue = Choices.Count - 1;
        }

        if (sliderValue > Choices.Count - 1)
        {
            sliderValue = 0;
        }

        OnValueChanged?.Invoke(this, Choices[sliderValue]);
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
